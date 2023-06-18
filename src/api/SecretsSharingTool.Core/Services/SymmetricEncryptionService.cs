using System.Security.Cryptography;

namespace SecretsSharingTool.Core.Services;

public class SymmetricEncryptionService
{
    public static async Task<SymmetricEncryptionResult> Encrypt(string message)
    {
        byte[] encrypted;
        
        var aes = Aes.Create();
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        // Create the streams used for encryption. 
        using (var msEncrypt = new MemoryStream())
        {
            await using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                await using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    //Write all data to the stream.
                    await swEncrypt.WriteAsync(message);
                }
                encrypted = msEncrypt.ToArray();
            }
        }

        return new SymmetricEncryptionResult()
        {
            EncryptedMessageBytes = encrypted,
            Key = aes.Key,
            Iv = aes.IV
        };
    }

    public static async Task<SymmetricDecryptionResult> Decrypt(byte[] ciphertext, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        await using var msDecrypt = new MemoryStream(ciphertext);
        await using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        var plaintext = await srDecrypt.ReadToEndAsync();

        return new SymmetricDecryptionResult()
        {
            Plaintext = plaintext
        };
    }
}

public class SymmetricEncryptionResult
{
    public byte[]? EncryptedMessageBytes { get; set; }
    public byte[]? Key { get; set; }
    public byte[]? Iv { get; set; }
}

public class SymmetricDecryptionResult
{
    public string? Plaintext { get; set; }
}
