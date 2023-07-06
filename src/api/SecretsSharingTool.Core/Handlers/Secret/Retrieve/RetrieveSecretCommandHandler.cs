using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecretsSharingTool.Core.Models;
using SecretsSharingTool.Core.Services;
using SecretsSharingTool.Data;

namespace SecretsSharingTool.Core.Handlers.Secret.Retrieve;

public class RetrieveSecretCommandHandler : BaseRequestHandler<RetrieveSecretCommand, RetrieveSecretCommandResponse>
{

    private const int NUMBER_OF_ALLOWED_ACCESSES = 5;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public RetrieveSecretCommandHandler(AppUnitOfWork unitOfWork, ILogger<RetrieveSecretCommandHandler> logger, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<RetrieveSecretCommandResponse> Handle(RetrieveSecretCommand request, CancellationToken cancellationToken)
    {
        var dbSecret = await Database.Secrets.FirstOrDefaultAsync(p => p.Id == request.SecretId, cancellationToken);

        var valid = await ValidateSecretState(request, dbSecret);

        if (!valid)
        {
            await Database.SaveChangesAsync(cancellationToken);
            
            return null;
        }
        
        SymmetricDecryptionResult? decryptedMessageResult = null;
        FailureReason? failureReason = null;

        try
        {
            var decryptedKeyResult = await AsymmetricEncryptionService.Decrypt(dbSecret.EncryptedSymmetricKey,
                Convert.FromBase64String(request.Key));

            decryptedMessageResult =
                await SymmetricEncryptionService.Decrypt(dbSecret.EncryptedMessage, decryptedKeyResult.Plaintext,
                    dbSecret.Iv);
            
            dbSecret.Deactivate();
        }
        catch (CryptographicException)
        {
            failureReason = FailureReason.DecryptionFailed;
            dbSecret!.NumberOfFailedAccesses++;
        }

        await CreateAuditRecordForState(dbSecret.Id, failureReason);
        await Database.SaveChangesAsync(cancellationToken);

        if (decryptedMessageResult is null)
        {
            return null;
        }

        return new RetrieveSecretCommandResponse()
        {
            Message = decryptedMessageResult.Plaintext
        };
    }

    private async Task<bool> ValidateSecretState(RetrieveSecretCommand request, Models.Secret? secret)
    {
        if (secret is null)
        {
            await CreateAuditRecordForState(request.SecretId, FailureReason.NotFound);
            
            return false;
        }

        if (!secret.IsActive)
        {
            await CreateAuditRecordForState(request.SecretId, FailureReason.SecretInactive);
            secret.Deactivate();
            
            return false;
        }

        if (secret.CreatedOn.UtcDateTime.AddMinutes(secret.ExpiryMinutes) < DateTime.UtcNow)
        {
            await CreateAuditRecordForState(request.SecretId, FailureReason.SecretExpired);
            secret.Deactivate();
            
            return false;
        }

        if (secret.NumberOfFailedAccesses >= NUMBER_OF_ALLOWED_ACCESSES)
        {
            await CreateAuditRecordForState(request.SecretId, FailureReason.NumberOfAllowedAttemptsExceeded);
            secret.Deactivate();

            return false;
        }

        return true;
    }

    private async Task CreateAuditRecordForState(Guid secretId, FailureReason? failureReason = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        await Database.AuditRecords.AddAsync(new Models.SecretAccessAudit()
        {
            SecretId = secretId,
            FailureReason = failureReason,
            ClientIpAddress = httpContext.Connection.RemoteIpAddress.ToString(),
            ClientUserAgent = httpContext.Request.Headers.FirstOrDefault(p => p.Key == "User-Agent").Value.ToString()
        });
    }
}
