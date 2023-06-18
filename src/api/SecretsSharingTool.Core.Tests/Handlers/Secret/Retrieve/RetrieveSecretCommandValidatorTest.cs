using SecretsSharingTool.Core.Handlers.Secret.Retrieve;

namespace SecretsSharingTool.Core.Tests.Handlers.Secret.Retrieve;

public class RetrieveSecretCommandValidatorTest : BaseValidatorTest<RetrieveSecretCommandValidator>
{
    [Fact]
    public async Task RetrieveSecretCommandValidator_WhenFieldsArePopulated_ValidationResultIsTrue()
    {
        var command = new RetrieveSecretCommand()
        {
            SecretId = Guid.NewGuid(),
            Key = "password"
        };

        var result = await Validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public async Task RetrieveSecretCommandValidator_WhenKeyLengthMeetsMaximum_ValidationResultIsTrue()
    {
        var command = new RetrieveSecretCommand()
        {
            SecretId = Guid.NewGuid(),
            Key = new string('A', 1000)
        };

        var result = await Validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public async Task RetrieveSecretCommandValidator_WhenSecretIdIsMissing_ValidationResultIsFalse()
    {
        var command = new RetrieveSecretCommand()
        {
            Key = "password"
        };

        var result = await Validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().ErrorMessage.Should().Be("'Secret Id' must not be empty.");
    }
    
    [Fact]
    public async Task RetrieveSecretCommandValidator_WhenKeyIsNull_ValidationResultIsFalse()
    {
        var command = new RetrieveSecretCommand()
        {
            SecretId = Guid.NewGuid(),
            Key = null
        };

        var result = await Validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().ErrorMessage.Should().Be("'Key' must not be empty.");
    }
    
    [Fact]
    public async Task RetrieveSecretCommandValidator_WhenKeyIsAnEmptyString_ValidationResultIsFalse()
    {
        var command = new RetrieveSecretCommand()
        {
            SecretId = Guid.NewGuid(),
            Key = ""
        };

        var result = await Validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().ErrorMessage.Should().Be("'Key' must not be empty.");
    }
    
    [Fact]
    public async Task RetrieveSecretCommandValidator_WhenKeyIsAllWhitespace_ValidationResultIsFalse()
    {
        var command = new RetrieveSecretCommand()
        {
            SecretId = Guid.NewGuid(),
            Key = "        "
        };

        var result = await Validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().ErrorMessage.Should().Be("'Key' must not be empty.");
    }
    
    [Fact]
    public async Task RetrieveSecretCommandValidator_WhenKeyLengthExceedsMaximum_ValidationResultIsFalse()
    {
        var command = new RetrieveSecretCommand()
        {
            SecretId = Guid.NewGuid(),
            Key = new string('A', 1000 + 1)
        };

        var result = await Validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().ErrorMessage.Should().Be("'Key Length' must be less than or equal to '1000'.");
    }
}
