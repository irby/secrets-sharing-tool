using Microsoft.EntityFrameworkCore;
using SecretsSharingTool.Core.Handlers.SecretAccessAudit.Cleanse;
using SecretsSharingTool.Core.Tests.Database;

namespace SecretsSharingTool.Core.Tests.Handlers.SecretAccessAudit.Cleanse;

public class CleanseSecretAccessAuditsCommandHandlerTests : BaseHandlerTest
{
    public CleanseSecretAccessAuditsCommandHandlerTests(DatabaseFixture fixture) : base(fixture)
    {
        MockDateTimeProvider.Setup(p => p.GetCurrentDateTimeOffset())
            .Returns(_testTime);
    }

    private readonly DateTimeOffset _testTime = DateTimeOffset.UtcNow;

    [Fact]
    public async Task Handle_WhenSecretAccessAuditsAreExpired_DeactivatesSecretAccessAudits()
    {
        var activeAudits = new List<Models.SecretAccessAudit>
        {
            NewSecretAccessAudit(_testTime),
            NewSecretAccessAudit(_testTime.AddDays(-180)),
        };

        var expiredAudits = new List<Models.SecretAccessAudit>
        {
            NewSecretAccessAudit(_testTime.AddDays(-180).AddMilliseconds(-1)),
            NewSecretAccessAudit(_testTime.AddDays(-181)),
        };

        await Database.AuditRecords.AddRangeAsync(activeAudits);
        await Database.AuditRecords.AddRangeAsync(expiredAudits);
        await Database.SaveChangesAsync();

        await Mediator.Send(new CleanseSecretAccessAuditsCommand());

        var emptySecrets = await Database.AuditRecords.ToListAsync();

        expiredAudits.ForEach(p =>
            emptySecrets.FirstOrDefault(x => x.Id == p.Id)
                .Should()
                .BeNull());
    }

    [Fact]
    public async Task Handle_WhenNoSecretAuditRecordsExpired_AllAuditRecordsArePresent()
    {
        var activeAudits = new List<Models.SecretAccessAudit>
        {
            NewSecretAccessAudit(_testTime),
            NewSecretAccessAudit(_testTime.AddDays(-180)),
        };

        await Database.AuditRecords.AddRangeAsync(activeAudits);
        await Database.SaveChangesAsync();

        await Mediator.Send(new CleanseSecretAccessAuditsCommand());

        var dbSecrets = await Database.AuditRecords.ToListAsync();
        activeAudits.ForEach(p =>
            dbSecrets.FirstOrDefault(x => x.Id == p.Id)
                .Should()
                .NotBeNull());
    }

    [Fact]
    public async Task Handle_WithLargeNumberOfSecretAuditRecordsToExpire_DeletesAllAuditRecords()
    {
        var auditRecordsList = new List<Models.SecretAccessAudit>();

        for (var i = 0; i < 105; i++)
        {
            auditRecordsList.Add(NewSecretAccessAudit(_testTime.AddDays(-181)));
        }

        await Database.AuditRecords.AddRangeAsync(auditRecordsList);
        await Database.SaveChangesAsync();

        await Mediator.Send(new CleanseSecretAccessAuditsCommand());

        var auditRecords = await Database.AuditRecords.ToListAsync();

        auditRecordsList.Count.Should().Be(105);

        auditRecordsList.ForEach(p => auditRecords.FirstOrDefault(q => q.Id == p.Id).Should().BeNull());
    }

    private Models.SecretAccessAudit NewSecretAccessAudit(DateTimeOffset createdOn) => new()
        { ClientIpAddress = "", ClientUserAgent = "", CreatedOn = createdOn };

}