using FluentValidation;
using SecretsSharingTool.Core.Handlers.Secret.Create;
using SecretsSharingTool.Core.Tests.Database;

namespace SecretsSharingTool.Core.Tests.Handlers.Secret.Create;

public class CreateSecretCommandHandlerTests : BaseHandlerTest
{
    public CreateSecretCommandHandlerTests(DatabaseFixture fixture) : base(fixture) { }
    
    [Fact]
    public async Task Handle_WhenCalled_CreatesSecretAndReturnsResponse()
    {
        var command = new CreateSecretCommand()
        {
            Message = "A bit of butter and salt is tasty",
            ExpireMinutes = 60
        };

        var now = DateTimeOffset.UtcNow;

        var response = await Mediator.Send(command);

        Database.Secrets.FirstOrDefault(p => p.Id == response.SecretId).Should().NotBeNull();

        response.SecretId.Should().NotBeEmpty();
        response.Key.Should().NotBeNull();
        response.ExpireDateTime.Should().BeOnOrAfter(now.AddMinutes(60));
        response.ExpireDateTimeEpoch.Should().BeGreaterOrEqualTo(now.AddMinutes(60).ToUnixTimeSeconds());
    }
    
    [Fact]
    public async Task CreateSecretCommandHandler_WhenInvalidCommandIsSupplied_ThrowsValidationException()
    {
        var command = new CreateSecretCommand()
        {
            Message = null,
            ExpireMinutes = 10
        };

        var ex = await Assert.ThrowsAsync<ValidationException>(() => Mediator.Send(command));
        ex.Message.Should().Be("'Message' must not be empty.");
    }
}
