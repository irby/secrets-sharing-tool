using Microsoft.EntityFrameworkCore;
using SecretsSharingTool.Core.Handlers.Secret.Cleanse;
using SecretsSharingTool.Core.Tests.Database;

namespace SecretsSharingTool.Core.Tests.Handlers.Secret.Cleanse;

public class CleanseSecretsCommandHandlerTests : BaseHandlerTest
{
    public CleanseSecretsCommandHandlerTests(DatabaseFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Handle_WhenSecretsAreExpired_DeactivatesSecrets()
    {
        var activeSecrets = new List<Models.Secret>
        {
            new ()
            {
                EncryptedMessage = new [] { (byte) 0 },
                IsActive = true,
                CreatedOn = DateTimeOffset.UtcNow,
                ExpiryMinutes = 10
            },
            new ()
            {
                EncryptedMessage = new [] { (byte) 1 },
                IsActive = true,
                CreatedOn = DateTimeOffset.UtcNow.AddMinutes(-9),
                ExpiryMinutes = 10
            },
        };
        
        var expiredSecrets = new List<Models.Secret>
        {
            new ()
            {
                EncryptedMessage = new [] { (byte) 2 },
                IsActive = false,
                CreatedOn = DateTimeOffset.UtcNow,
                ExpiryMinutes = 10
            },
            new ()
            {
                EncryptedMessage = new [] { (byte) 3 },
                IsActive = true,
                CreatedOn = DateTimeOffset.UtcNow.AddMinutes(-11),
                ExpiryMinutes = 10
            },
        };

        await Database.Secrets.AddRangeAsync(activeSecrets);
        await Database.Secrets.AddRangeAsync(expiredSecrets);
        await Database.SaveChangesAsync();

        await Mediator.Send(new CleanseSecretsCommand());

        var emptySecrets = await Database.Secrets.Where(p => p.EncryptedMessage == null).ToListAsync();
        
        expiredSecrets.ForEach(p => 
            emptySecrets.FirstOrDefault(x => x.Id == p.Id)
                .Should()
                .NotBeNull());
    }
    
    [Fact]
    public async Task Handle_WhenNoSecretsExpired_AllSecretsAreActive()
    {
        var activeSecrets = new List<Models.Secret>
        {
            new ()
            {
                EncryptedMessage = new [] { (byte) 0 },
                IsActive = true,
                CreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiryMinutes = 10
            },
            new ()
            {
                EncryptedMessage = new [] { (byte) 1 },
                IsActive = true,
                CreatedOn = DateTimeOffset.UtcNow.AddMinutes(-9),
                ExpiryMinutes = 10
            },
        };

        await Database.Secrets.AddRangeAsync(activeSecrets);
        await Database.SaveChangesAsync();

        await Mediator.Send(new CleanseSecretsCommand());

        var dbSecrets = await Database.Secrets.Where(p => p.EncryptedMessage != null).ToListAsync();
        dbSecrets.ForEach(p => p.IsActive.Should().BeTrue());
        activeSecrets.ForEach(p => 
            dbSecrets.FirstOrDefault(x => x.Id == p.Id)
                .Should()
                .NotBeNull());
    }
    
    [Fact]
    public async Task Handle_WithLargeNumberOfSecretsToExpire_ExpiresAllSecrets()
    {
        var secretsList = new List<Models.Secret>();
        
        for (var i = 0; i < 105; i++)
        {
            secretsList.Add(new Models.Secret()
            {
                EncryptedMessage = new [] { (byte) 0 },
                IsActive = false,
                CreatedOn = DateTimeOffset.UtcNow,
                ExpiryMinutes = 10
            });
        }

        await Database.Secrets.AddRangeAsync(secretsList);
        await Database.SaveChangesAsync();

        await Mediator.Send(new CleanseSecretsCommand());

        var dbSecrets = await Database.Secrets.Where(p => p.EncryptedMessage == null).ToListAsync();

        dbSecrets.Count(p => secretsList.Select(p => p.Id).Contains(p.Id))
            .Should()
            .Be(105);
    }
}