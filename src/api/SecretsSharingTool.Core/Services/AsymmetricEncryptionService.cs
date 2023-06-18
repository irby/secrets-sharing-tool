using System.Security.Cryptography;

namespace SecretsSharingTool.Core.Services;

public class AsymmetricEncryptionService
{
    private const int KeySize = 1024;
    private static readonly RSAEncryptionPadding Padding = RSAEncryptionPadding.Pkcs1;
    
    public static async Task<AsymmetricEncryptionResult> Encrypt(byte[] plaintext)
    {
        using var rsa = RSA.Create(KeySize);

        var encrypted = rsa.Encrypt(plaintext, Padding);

        return new AsymmetricEncryptionResult()
        {
            Ciphertext = encrypted,
            Key = rsa.ExportRSAPrivateKey()
        };
    }

    public static async Task<AsymmetricDecryptionResult> Decrypt(byte[] ciphertext, byte[] key)
    {
        using var rsa = RSA.Create(KeySize);
        
        rsa.ImportRSAPrivateKey(key, out _);

        var decrypted = rsa.Decrypt(ciphertext, Padding);

        return new AsymmetricDecryptionResult()
        {
            Plaintext = decrypted
        };
    }
}

public class AsymmetricEncryptionResult
{
    public byte[] Ciphertext { get; set; }
    public byte[] Key { get; set; }
}

public class AsymmetricDecryptionResult
{
    public byte[] Plaintext { get; set; }
}
