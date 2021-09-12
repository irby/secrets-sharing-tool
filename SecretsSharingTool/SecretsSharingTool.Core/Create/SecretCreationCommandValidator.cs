using FluentValidation;

namespace SecretsSharingTool.Core.Create
{
    public sealed class SecretCreationCommandValidator : AbstractValidator<SecretCreationCommand>
    {
        public SecretCreationCommandValidator()
        {
            RuleFor(p => p.Message).NotEmpty();
            RuleFor(p => p.Message).MaximumLength(100);
            RuleFor(p => p.SecondsToLive).GreaterThan(0);
        }
    }
}