using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Moq;
using SecretsSharingTool.Core.Handlers.Secret.Create;
using SecretsSharingTool.Core.Handlers.Secret.Retrieve;
using SecretsSharingTool.Core.Models;
using SecretsSharingTool.Core.Tests.Database;

namespace SecretsSharingTool.Core.Tests.Handlers.Secret.Retrieve;

public class RetrieveSecretCommandHandlerTests : BaseHandlerTest
{
    private readonly HttpContext _mockHttpContext = CreateFakeHttpContext();
    public RetrieveSecretCommandHandlerTests(DatabaseFixture fixture) : base(fixture) { }
    
    [Fact]
    public async Task Handle_WhenSecretIdAndKeyAreValid_RetrievesSecretAndDeactivatesSecretWithAuditLog()
    {
        var createSecretResponse = await CreateSecret("hello world", 1);

        var retrieveSecretResponse = await Mediator.Send(CreateCommand(createSecretResponse));

        retrieveSecretResponse.Message.Should().Be("hello world");

        var dbSecret = await Database.Secrets.FirstAsync(p => p.Id == createSecretResponse.SecretId);
        dbSecret.IsActive.Should().BeFalse();
        dbSecret.EncryptedMessage.Should().BeNull();

        var auditRecord = await Database.AuditRecords.FirstAsync(p => p.SecretId == createSecretResponse.SecretId);
        auditRecord.IsSuccessful.Should().BeTrue();
        auditRecord.FailureReason.Should().BeNull();
        TestAuditRecordUserAgentAndIpAddress(auditRecord);
    }
    
    [Fact]
    public async Task Handle_WhenSecretIdDoesNotExist_ReturnsNullWithAuditLog()
    {
        var createSecretResponse = await CreateSecret("hello world", 1);
        createSecretResponse.SecretId = Guid.NewGuid();

        var retrieveSecretResponse = await Mediator.Send(CreateCommand(createSecretResponse));

        retrieveSecretResponse.Should().BeNull();

        var dbSecret = await Database.Secrets.FirstOrDefaultAsync(p => p.Id == createSecretResponse.SecretId);
        dbSecret.Should().BeNull();

        var auditRecord = await Database.AuditRecords.FirstAsync(p => p.SecretId == createSecretResponse.SecretId);
        auditRecord.IsSuccessful.Should().BeFalse();
        auditRecord.FailureReason.Should().Be(FailureReason.NotFound);
        TestAuditRecordUserAgentAndIpAddress(auditRecord);
    }
    
    [Fact]
    public async Task Handle_WhenKeyIsInvalid_ReturnsNullWithAuditLog()
    {
        var createSecretResponse = await CreateSecret("hello world", 1);
        createSecretResponse.Key = "somethinginvalid";

        var retrieveSecretResponse = await Mediator.Send(CreateCommand(createSecretResponse));

        retrieveSecretResponse.Should().BeNull();

        var dbSecret = await Database.Secrets.FirstOrDefaultAsync(p => p.Id == createSecretResponse.SecretId);
        dbSecret.Should().NotBeNull();

        var auditRecord = await Database.AuditRecords.FirstAsync(p => p.SecretId == createSecretResponse.SecretId);
        auditRecord.IsSuccessful.Should().BeFalse();
        auditRecord.FailureReason.Should().Be(FailureReason.DecryptionFailed);
        TestAuditRecordUserAgentAndIpAddress(auditRecord);
    }
    
    [Fact]
    public async Task Handle_WhenKeyIsValidButDecryptionFails_ReturnsNullWithAuditLog()
    {
        var createSecretResponse = await CreateSecret("hello world", 1);
        
        var dbSecret = await Database.Secrets.FirstAsync(p => p.Id == createSecretResponse.SecretId);
        dbSecret.EncryptedMessage = new[] { new byte() };
        await Database.SaveChangesAsync();

        var retrieveSecretResponse = await Mediator.Send(CreateCommand(createSecretResponse));
        
        dbSecret = await Database.Secrets.FirstOrDefaultAsync(p => p.Id == createSecretResponse.SecretId);
        dbSecret.Should().NotBeNull();

        retrieveSecretResponse.Should().BeNull();
        
        var auditRecord = await Database.AuditRecords.FirstAsync(p => p.SecretId == createSecretResponse.SecretId);
        auditRecord.IsSuccessful.Should().BeFalse();
        auditRecord.FailureReason.Should().Be(FailureReason.DecryptionFailed);
        TestAuditRecordUserAgentAndIpAddress(auditRecord);
    }
    
    [Fact]
    public async Task Handle_WhenSecretIsExpired_ReturnsNullWithAuditLog()
    {
        var createSecretResponse = await CreateSecret("hello world", 1);

        var createdSecret = await Database.Secrets.FirstAsync(p => p.Id == createSecretResponse.SecretId);
        createdSecret.ExpiryMinutes = -1;
        await Database.SaveChangesAsync();

        var retrieveSecretResponse = await Mediator.Send(CreateCommand(createSecretResponse));

        retrieveSecretResponse.Should().BeNull();

        var dbSecret = await Database.Secrets.FirstOrDefaultAsync(p => p.Id == createSecretResponse.SecretId);
        dbSecret.EncryptedMessage.Should().BeNull();

        var auditRecord = await Database.AuditRecords.FirstAsync(p => p.SecretId == createSecretResponse.SecretId);
        auditRecord.IsSuccessful.Should().BeFalse();
        auditRecord.FailureReason.Should().Be(FailureReason.SecretExpired);
        TestAuditRecordUserAgentAndIpAddress(auditRecord);
    }
    
    [Fact]
    public async Task Handle_WhenSecretIsInactive_ReturnsNullWithAuditLog()
    {
        var createSecretResponse = await CreateSecret("hello world", 1);
        
        var createdSecret = await Database.Secrets.FirstAsync(p => p.Id == createSecretResponse.SecretId);
        createdSecret.IsActive = false;
        await Database.SaveChangesAsync();

        var retrieveSecretResponse = await Mediator.Send(CreateCommand(createSecretResponse));

        retrieveSecretResponse.Should().BeNull();

        var dbSecret = await Database.Secrets.FirstOrDefaultAsync(p => p.Id == createSecretResponse.SecretId);
        dbSecret.EncryptedMessage.Should().BeNull();

        var auditRecord = await Database.AuditRecords.FirstAsync(p => p.SecretId == createSecretResponse.SecretId);
        auditRecord.IsSuccessful.Should().BeFalse();
        auditRecord.FailureReason.Should().Be(FailureReason.SecretInactive);
        TestAuditRecordUserAgentAndIpAddress(auditRecord);
    }
    
    [Fact]
    public async Task Handle_WhenNumberOfAllowedAttemptsIsExceeded_ReturnsNullWithAuditLog()
    {
        var createSecretResponse = await CreateSecret("hello world", 1);
        createSecretResponse.Key = "somethinginvalid";

        RetrieveSecretCommandResponse? retrieveSecretResponse;
        Models.Secret secret;

        for (var i = 0; i < 5; i++)
        {
            retrieveSecretResponse = await Mediator.Send(CreateCommand(createSecretResponse));
            retrieveSecretResponse.Should().BeNull();
            secret = await Database.Secrets.SingleAsync(p => p.Id == createSecretResponse.SecretId);
            secret.IsActive.Should().BeTrue();
            secret.EncryptedMessage.Should().NotBeNull();
        }
        
        retrieveSecretResponse = await Mediator.Send(CreateCommand(createSecretResponse));
        retrieveSecretResponse.Should().BeNull();
        
        var auditRecord = await Database.AuditRecords
            .OrderByDescending(p => p.CreatedOn)
            .FirstAsync(p => p.SecretId == createSecretResponse.SecretId);
        auditRecord.IsSuccessful.Should().BeFalse();
        auditRecord.FailureReason.Should().Be(FailureReason.NumberOfAllowedAttemptsExceeded);
        TestAuditRecordUserAgentAndIpAddress(auditRecord);
        
        secret = await Database.Secrets.SingleAsync(p => p.Id == createSecretResponse.SecretId);
        secret.IsActive.Should().BeFalse();
        secret.EncryptedMessage.Should().BeNull();
    }
    
    [Fact]
    public async Task RetrieveSecretCommandHandler_WhenInvalidDataIsSupplied_ThrowsValidationException()
    {
        var command = new RetrieveSecretCommand()
        {
            SecretId = Guid.NewGuid(),
            Key = new string('A', 1000 + 1)
        };

        var ex = await Assert.ThrowsAsync<ValidationException>(() => Mediator.Send(command));
        ex.Message.Should().Be("'Key Length' must be less than or equal to '1000'.");
    }

    private async Task<CreateSecretCommandResponse> CreateSecret(string message, long expireMinutes)
    {
        var command = new CreateSecretCommand()
        {
            Message = message,
            ExpireMinutes = expireMinutes
        };

        var response = await Mediator.Send(command);
        return response;
    }

    private RetrieveSecretCommand CreateCommand(CreateSecretCommandResponse response)
    {
        return new RetrieveSecretCommand()
        {
            SecretId = response.SecretId,
            Key = response.Key
        };
    }

    protected override void ConfigureAdditionalServices()
    {
        var mockHttpContext = new Mock<IHttpContextAccessor>();
        mockHttpContext.Setup(p => p.HttpContext).Returns(_mockHttpContext);
        ServiceCollection.AddTransient(_ => mockHttpContext.Object);
    }

    private static HttpContext CreateFakeHttpContext()
    {
        return new DefaultHttpContext()
        {
            Connection =
            {
                RemoteIpAddress = new IPAddress(69832301)
            },
            Request =
            {
                Headers = { new KeyValuePair<string, StringValues>("User-Agent", "Test Browser") }
            }
        };
    }

    private void TestAuditRecordUserAgentAndIpAddress(SecretAccessAudit audit)
    {
        audit.ClientUserAgent.Should().Be("Test Browser");
        audit.ClientIpAddress.Should().Be("109.142.41.4");
    }
}
