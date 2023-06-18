using System.Text;
using SecretsSharingTool.Core.Services;

namespace SecretsSharingTool.Core.Tests.Services;

public class AsymmetricEncryptionServiceTests
{
    [Fact]
    public async Task Encrypt_WhenProvidedAPlaintext_EncryptsContents()
    {
        var plaintext = "Hello, World!";
        var bytes = Encoding.UTF8.GetBytes(plaintext);

        var result = await AsymmetricEncryptionService.Encrypt(bytes);

        result.Ciphertext.Should().NotBeEmpty();
        result.Key.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task Encrypt_WhenEncryptedTwice_ProducesDifferentByteArrays()
    {
        var plaintext = "Hello, World!";
        var bytes = Encoding.UTF8.GetBytes(plaintext);

        var result1 = await AsymmetricEncryptionService.Encrypt(bytes);
        var result2 = await AsymmetricEncryptionService.Encrypt(bytes);

        result1.Ciphertext.Should().NotEqual(result2.Ciphertext);
        result1.Key.Should().NotEqual(result2.Key);
    }
    
    [Fact]
    public async Task Decrypt_WhenProvidedCiphertextAndKey_DecryptsToPlaintext()
    {
        var ciphertext =
            Convert.FromBase64String(
                "q+KM3OTlzyCADyxG3elpG6qlTDSQnccuZE7fPVAhPnABy5ZV2GzvNUxpvh7i37FjJcBtwmpPlNs8nsGwUPW3Ogzea2705kOV8Szk+p4Fe9K1zNZgSrG2Kd9qsuyjUICwfkHQohs7eCx324ac2R9IItQzu2Btf3DQfjIoUSgbzjU=");
        var key = Convert.FromBase64String(
            "MIICXAIBAAKBgQC2cGxT+6vO1+m0U0EMsmVngm2VbXJPhiu2IS/Xkb49fyykSn3DLCraSzN/yc7ZHiqeg6Pt1DhX+SpSukmjkE2TnCvLi0iQ+Vhk2sxt5Ejm6zNMEeGvOH1MM3F5ulocxtwncEo9Nb6fzwk/shXt1AXNZ7n8ShrBErz8T3/Wb60tIQIDAQABAoGAO+TZs5Kmm3AH8iL48p2oq9xGdK2rPw9Zz1/C0J8w2Qik08T+17Hq2aKhpBzRUzdTn5mxGjW3T65l5iXxYgjQZoy/QF6dOCtE2umKlVNs8lb92OLxJPeLv/w6u+6EW02pYjJcmIvXaieDU5QI7gBRREyXFjoY13OmciXzLNTif28CQQDnTWPJ+bxteZiEowJp7wtEPb+RWDKnPhnM61eu9UiUjMAxTHwblQEORO3ZRLA+egmTksddCMffnpt0GLh9+O+HAkEAyeteAYdKbg15NCTylxCGaCa4bXOf1oE+mLoFHdO2U0vXZ0UQAFBTXx3v92bvE1m5j+Y7VMadRdL8jgYO8g0YFwJANYIguKPOZGEB2IVBVLJZH+NNAtmtdiUWoOI8uZvCu6LH/1+bQmI5CU0G3QFX6EwhFQOanTofyuqNOdFSNMo21wJAWH1GsGLLEpnjASAkFGIQlTpK3uSqKsZvWV1EesLah3yYRNC2Z3zMXMXw8TpyEcfjk5WxcMCuEfiZ59/t0tQ8NwJBAIRqti1KBsRlBTFz7cEw2Khv/1LaUTTkQPcUG6sx4PAaweW4F3NX5q6OT3k4y4uvuwPEx9QXalv/6NLFoblW4Ko=");

        var result = await AsymmetricEncryptionService.Decrypt(ciphertext, key);

        Encoding.UTF8.GetString(result.Plaintext).Should().Be("Hello, World!");
    }
    
    [Fact]
    public async Task Encrypt_Decrypt_ConvertsPlaintextToCiphertextBackToPlaintext()
    {
        var message = "The quick brown fox jumps over a lazy dog";

        var encryptionResult = await AsymmetricEncryptionService.Encrypt(Encoding.UTF8.GetBytes(message));
        var decryptionResult = await AsymmetricEncryptionService.Decrypt(encryptionResult.Ciphertext, encryptionResult.Key);

        Encoding.UTF8.GetString(decryptionResult.Plaintext).Should().Be(message);
    }
}
