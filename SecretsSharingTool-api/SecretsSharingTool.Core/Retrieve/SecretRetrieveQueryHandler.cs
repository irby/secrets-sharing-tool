using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SecretSharingTool.Data.Database;
using SecretSharingTool.Data.Models;
using SecretsSharingTool.Core.Shared;

namespace SecretsSharingTool.Core.Retrieve
{
    public sealed class SecretRetrieveQueryHandler : IRequestHandler<SecretRetrieveQuery, SecretRetrieveQueryResponse>
    {
        public SecretRetrieveQueryHandler(AppUnitOfWork appUnitOfWork)
        {
            _appUnitOfWork = appUnitOfWork;
        }

        private readonly AppUnitOfWork _appUnitOfWork;
        public int NumberOfAllowedAttempts { get; private set; } = 5;

        /// <summary>
        /// Provided a GUID and a private key, this method will look for a valid and active record with the GUID and attempt to
        /// decrypt the message stored using the private key supplied.
        /// </summary>
        /// <param name="query">A query containing the GUID and private key related to a record</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns null if the secret was not found, could not be decrypted, or validation failed. Returns a decrypted message if decryption and validation was successful.</returns>
        public async Task<SecretRetrieveQueryResponse> Handle(SecretRetrieveQuery query, CancellationToken cancellationToken)
        {
            var secret = await _appUnitOfWork.Secrets.SingleOrDefaultAsync(p => p.Id == query.Id && p.IsActive && p.ExpireDate >= DateTime.UtcNow, cancellationToken);

            if (secret == null)
                return null;

            try
            {
                // If secret has had more than allowed number of accesses, inactivate the record and return
                if (secret.NumberOfAttempts >= NumberOfAllowedAttempts)
                {
                    Console.WriteLine($"Number of allowed accesses has passed for record {secret.Id}");
                    secret.SetModifiedAndInactive();
                    await _appUnitOfWork.SaveChangesAsync(cancellationToken);
                    return null;
                }

                secret.NumberOfAttempts++;
                await _appUnitOfWork.SaveChangesAsync(cancellationToken);
                
                var rsa = RSA.Create(1024);
                rsa.ImportRSAPrivateKey(query.PrivateKey, out var privateKeyBytesRead);

                var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                rsaDeformatter.SetHashAlgorithm("SHA256");
                
                var message = rsa.Decrypt(secret.Message, RSAEncryptionPadding.Pkcs1);

                var sha = SHA256.Create();
                var hash = await Task.Run(() => sha.ComputeHash(message), cancellationToken);

                // Validate the decrypted message was the original message passed in
                if (!rsaDeformatter.VerifySignature(hash, secret.SignedHash))
                {
                    throw new Exception("Signature is not valid");
                }

                secret.SetModifiedAndInactive();
                secret.ModifiedOn = DateTime.UtcNow;

                await _appUnitOfWork.SaveChangesAsync(cancellationToken);

                return new SecretRetrieveQueryResponse()
                {
                    Message = System.Text.Encoding.Unicode.GetString(message)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} {1}", secret.Id, ex);
            }

            return null;
        }
    }
}