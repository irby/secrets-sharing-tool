using FluentValidation;
using SecretsSharingTool.Core.Constants;

namespace SecretsSharingTool.Core.Handlers.Secret.Create;

public class CreateSecretCommandValidator : AbstractValidator<CreateSecretCommand>
{
    public CreateSecretCommandValidator()
    {
        RuleFor(p => p.Message).NotEmpty();
        RuleFor(p => p.Message.Length).LessThanOrEqualTo(SecretConstants.SecretLengthMaximum)
            .When(p => p.Message != null);
        RuleFor(p => p.ExpireMinutes).GreaterThan(0);
        RuleFor(p => p.ExpireMinutes).LessThan(60 * 24 * 7);
    }
}
