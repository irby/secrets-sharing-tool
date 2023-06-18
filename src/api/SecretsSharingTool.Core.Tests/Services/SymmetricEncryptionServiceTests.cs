using SecretsSharingTool.Core.Services;

namespace SecretsSharingTool.Core.Tests.Services;

public class SymmetricEncryptionServiceTests
{
    [Fact]
    public async Task Encrypt_WhenProvidedAMessage_EncryptsMessageAsync()
    {
        var message = "Hello World!";

        var result = await SymmetricEncryptionService.Encrypt(message);

        result.EncryptedMessageBytes.Should().NotBeEmpty();
        result.Key.Should().NotBeEmpty();
        result.Iv.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task Encrypt_WhenEncryptedTwice_ProducesDifferentByteArrays()
    {
        var message = "Hello World!";

        var result1 = await SymmetricEncryptionService.Encrypt(message);
        var result2 = await SymmetricEncryptionService.Encrypt(message);

        result1.EncryptedMessageBytes.Should().NotEqual(result2.EncryptedMessageBytes);
        result1.Iv.Should().NotEqual(result2.Iv);
        result1.Key.Should().NotEqual(result2.Key);
    }

    [Fact]
    public async Task Decrypt_WhenProvidedCiphertextKeyAndIv_DecryptsToPlaintext()
    {
        var ciphertext = Convert.FromBase64String("wsBZcHJD4N3Lbpyo2UB6OQ==");
        var key = Convert.FromBase64String("EYx3J2hWGYv9bCGKz7UkYMaQfBocqnpj9Evt4MpFaos=");
        var iv = Convert.FromBase64String("Nqm2WchpJWjSMltI77t6ow==");

        var result = await SymmetricEncryptionService.Decrypt(ciphertext, key, iv);

        result.Plaintext.Should().Be("Hello World!");
    }

    [Fact]
    public async Task Encrypt_Decrypt_ConvertsPlaintextToCiphertextBackToPlaintext()
    {
        var message = "The quick brown fox jumps over a lazy dog";

        var encryptionResult = await SymmetricEncryptionService.Encrypt(message);

        var decryptionResult = await SymmetricEncryptionService.Decrypt(encryptionResult.EncryptedMessageBytes, encryptionResult.Key, encryptionResult.Iv);

        decryptionResult.Plaintext.Should().Be(message);
    }
}