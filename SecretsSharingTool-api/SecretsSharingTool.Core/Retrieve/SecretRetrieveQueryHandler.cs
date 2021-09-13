using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SecretSharingTool.Data.Database;
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
            var secret = await _appUnitOfWork.Secrets.SingleOrDefaultAsync(p => p.Id == query.Id && p.IsActive && p.ExpireDateTime >= DateTime.UtcNow, cancellationToken);

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
                
                var key = rsa.Decrypt(secret.EncryptedSymmetricKey, RSAEncryptionPadding.Pkcs1);

                var message = DecryptStringFromBytes(secret.Message, key, secret.Iv);
                
                var sha = SHA256.Create();
                var hash = await Task.Run(() => sha.ComputeHash(secret.Message), cancellationToken);

                // Validate the decrypted message was the original message passed in
                if (!rsaDeformatter.VerifySignature(hash, secret.SignedHash))
                {
                    throw new Exception("Signature is not valid");
                }
                
                secret.SetModifiedAndInactive();
                await _appUnitOfWork.SaveChangesAsync(cancellationToken);

                return new SecretRetrieveQueryResponse()
                {
                    Message = message
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} {1}", secret.Id, ex);
            }

            return null;
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            string plaintext = null;

            using var rijAlg = new RijndaelManaged();
            rijAlg.Key = key;
            rijAlg.IV = iv;

            var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                
            // Create the streams used for decryption. 
            using var msDecrypt = new MemoryStream(cipherText);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            // Read the decrypted bytes from the decrypting stream 
            // and place them in a string.
            plaintext = srDecrypt.ReadToEnd();

            return plaintext;
        }
    }
}