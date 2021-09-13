﻿using System;
using System.IO;
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
        /// Using the Rijndael and RSA encryption schemes, encrypt and sign the message and store into a database. Return the private key and GUID back to the caller.
        /// </summary>
        /// <param name="request">A request containing the message to be encrypted as well as the time to live in seconds.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The GUID and private key associated to the secret.</returns>
        public async Task<SecretCreationCommandHandlerResponse> Handle(SecretCreationCommand request, CancellationToken cancellationToken)
        {
            var (encryptedMessage, key, iv) = EncryptStringToBytes(request.Message);
            
            var rsa = RSA.Create(1024);

            var encryptedKey = rsa.Encrypt(key, RSAEncryptionPadding.Pkcs1);

            var sha = SHA256.Create();
            var hash = await Task.Run(() => sha.ComputeHash(encryptedMessage), cancellationToken);
            
            // Sign the message to ensure integrity of the message stored
            var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
            rsaFormatter.SetHashAlgorithm("SHA256");

            var secret = new Secret()
            {
                Message = encryptedMessage,
                EncryptedSymmetricKey = encryptedKey,
                Iv = iv,
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

        private static (byte[], byte[], byte[]) EncryptStringToBytes(string plainText)
        {
            byte[] encrypted;
            byte[] key;
            byte[] iv;
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.GenerateKey();
                rijAlg.GenerateIV();

                key = rijAlg.Key;
                iv = rijAlg.IV;

                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                
                // Create the streams used for encryption. 
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return (encrypted, key, iv);
        }
    }
}