using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SecretSharingTool.Data.Database;
using SecretSharingTool.Data.Models;
using SecretsSharingTool.Core.Shared;

namespace SecretsSharingTool.Core.Create
{
    public sealed class SecretCreationCommandHandler : IRequestHandler<SecretCreationCommand, SecretCreationCommandHandlerResponse>
    {
        public SecretCreationCommandHandler(AppUnitOfWork appUnitOfWork)
        {
            _appUnitOfWork = appUnitOfWork;
        }

        private readonly AppUnitOfWork _appUnitOfWork;
        
        /// <summary>
        /// Using an RSA encryption scheme, encrypt and sign the message and store into a database. Return the private key and GUID back to the caller.
        /// </summary>
        /// <param name="request">A request containing the message to be encrypted as well as the time to live in seconds.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The GUID and private key associated to the secret.</returns>
        public async Task<SecretCreationCommandHandlerResponse> Handle(SecretCreationCommand request, CancellationToken cancellationToken)
        {
            var rsa = RSA.Create(1024);

            var cipherText = rsa.Encrypt(System.Text.Encoding.Unicode.GetBytes(request.Message), RSAEncryptionPadding.Pkcs1);

            var sha = SHA256.Create();
            var hash = await Task.Run(() => sha.ComputeHash(System.Text.Encoding.Unicode.GetBytes(request.Message)), cancellationToken);
            
            // Sign the message to ensure integrity of the message stored
            var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
            rsaFormatter.SetHashAlgorithm("SHA256");

            var secret = new Secret()
            {
                Message = cipherText,
                PublicKey = rsa.ExportRSAPublicKey(),
                SignedHash = rsaFormatter.CreateSignature(hash),
                ExpireDate = DateTime.UtcNow.AddSeconds(request.SecondsToLive)
            };
            
            secret.SetCreatedAndActive();

            await _appUnitOfWork.Secrets.AddAsync(secret, cancellationToken);
            await _appUnitOfWork.SaveChangesAsync(cancellationToken);

            return new SecretCreationCommandHandlerResponse()
            {
                Id = secret.Id,
                Key = Convert.ToBase64String(rsa.ExportRSAPrivateKey())
            };
        }
    }
}