using FluentValidation;

namespace SecretsSharingTool.Core.Handlers.Secret.Retrieve;

public class RetrieveSecretCommandValidator : AbstractValidator<RetrieveSecretCommand>
{
    public RetrieveSecretCommandValidator()
    {
        RuleFor(p => p.SecretId).NotEmpty();
        RuleFor(p => p.Key).NotEmpty();
        RuleFor(p => p.Key.Length).LessThanOrEqualTo(1000)
            .When(p => p.Key != null);
    }
}
