using SecretsSharingTool.Core.Constants;
using SecretsSharingTool.Core.Handlers.Secret.Create;

namespace SecretsSharingTool.Core.Tests.Handlers.Secret.Create;

public class CreateSecretCommandValidatorTest : BaseValidatorTest<CreateSecretCommandValidator>
{
    [Fact]
    public async Task CreateSecretCommandValidator_WhenMessageIsNull_ValidationResultIsFalse()
    {
        var command = new CreateSecretCommand()
        {
            Message = null,
            ExpireMinutes = 10
        };

        var result = await Validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().ErrorMessage.Should().Be("'Message' must not be empty.");
    }
    
    [Fact]
    public async Task CreateSecretCommandValidator_WhenMessageIsAnEmptyString_ValidationResultIsFalse()
    {
        var command = new CreateSecretCommand()
        {
            Message = "",
            ExpireMinutes = 10
        };
        
        var result = await Validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().ErrorMessage.Should().Be("'Message' must not be empty.");
    }
    
    [Fact]
    public async Task CreateSecretCommandValidator_WhenMessageIsAStringWithWhitespace_ValidationResultIsFalse()
    {
        var command = new CreateSecretCommand()
        {
            Message = "    ",
            ExpireMinutes = 10
        };
        
        var result = await Validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().ErrorMessage.Should().Be("'Message' must not be empty.");
    }
    
    [Fact]
    public async Task CreateSecretCommandValidator_WhenMessageLengthExceedsMaximum_ValidationResultIsFalse()
    {
        var command = new CreateSecretCommand()
        {
            Message = new string('A', SecretConstants.SecretLengthMaximum + 1),
            ExpireMinutes = 10
        };
        
        var result = await Validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().ErrorMessage.Should().Be("'Message Length' must be less than or equal to '5000'.");
    }
    
    [Fact]
    public async Task CreateSecretCommandValidator_WhenExpireMinutesIsZero_ValidationResultIsFalse()
    {
        var command = new CreateSecretCommand()
        {
            Message = "abc",
            ExpireMinutes = 0
        };
        
        var result = await Validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().ErrorMessage.Should().Be("'Expire Minutes' must be greater than '0'.");
    }
    
    [Fact]
    public async Task CreateSecretCommandValidator_WhenExpireMinutesIsNegative_ValidationResultIsFalse()
    {
        var command = new CreateSecretCommand()
        {
            Message = "abc",
            ExpireMinutes = -1
        };
        
        var result = await Validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().ErrorMessage.Should().Be("'Expire Minutes' must be greater than '0'.");
    }
    
    [Fact]
    public async Task CreateSecretCommandValidator_WhenExpireMinutesMeetsMaximum_ValidationResultIsFalse()
    {
        var command = new CreateSecretCommand()
        {
            Message = "abc",
            ExpireMinutes = 24 * 60 * 7
        };
        
        var result = await Validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().ErrorMessage.Should().Be("'Expire Minutes' must be less than '10080'.");
    }
    
    [Fact]
    public async Task CreateSecretCommandValidator_WhenExpireMinutesExceedsMaximum_ValidationResultIsFalse()
    {
        var command = new CreateSecretCommand()
        {
            Message = "abc",
            ExpireMinutes = (24 * 60 * 7) + 1
        };
        
        var result = await Validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().ErrorMessage.Should().Be("'Expire Minutes' must be less than '10080'.");
    }
    
    [Fact]
    public async Task CreateSecretCommandValidator_WhenValuesAreValid_ValidationResultIsTrue()
    {
        var command = new CreateSecretCommand()
        {
            Message = "abc",
            ExpireMinutes = 10
        };
        
        var result = await Validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public async Task CreateSecretCommandValidator_WhenMessageLengthMeetsMaximum_ValidationResultIsTrue()
    {
        var command = new CreateSecretCommand()
        {
            Message = new string('A', SecretConstants.SecretLengthMaximum),
            ExpireMinutes = 10
        };
        
        var result = await Validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }
}
