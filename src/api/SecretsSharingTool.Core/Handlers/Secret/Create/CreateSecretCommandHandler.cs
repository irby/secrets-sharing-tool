using Microsoft.Extensions.Logging;
using SecretsSharingTool.Core.Services;
using SecretsSharingTool.Data;

namespace SecretsSharingTool.Core.Handlers.Secret.Create;

public class CreateSecretCommandHandler : BaseRequestHandler<CreateSecretCommand, CreateSecretCommandResponse>
{
    public CreateSecretCommandHandler(AppUnitOfWork unitOfWork, ILogger<CreateSecretCommand> logger) : base(unitOfWork, logger)
    {
    }
    
    public override async Task<CreateSecretCommandResponse> Handle(CreateSecretCommand request, CancellationToken cancellationToken)
    {
        var encryptedMessageResult = await SymmetricEncryptionService.Encrypt(request.Message);
        var encryptedKeyResult = await AsymmetricEncryptionService.Encrypt(encryptedMessageResult.Key);

        var secret = new Models.Secret
        {
            EncryptedMessage = encryptedMessageResult.EncryptedMessageBytes,
            EncryptedSymmetricKey = encryptedKeyResult.Ciphertext,
            Iv = encryptedMessageResult.Iv,
            ExpiryMinutes = request.ExpireMinutes
        };

        await Database.Secrets.AddAsync(secret, cancellationToken);
        await Database.SaveChangesAsync(cancellationToken);

        return new CreateSecretCommandResponse()
        {
            SecretId = secret.Id,
            ExpireDateTime = secret.CreatedOn.UtcDateTime.AddMinutes(secret.ExpiryMinutes),
            Key = Convert.ToBase64String(encryptedKeyResult.Key)
        };
    }

    
}
