using FluentValidation;

namespace SecretsSharingTool.Core.Create
{
    public sealed class SecretCreationCommandValidator : AbstractValidator<SecretCreationCommand>
    {
        private const int MaxNumberOfDaysAllowed = 7;
        public SecretCreationCommandValidator()
        {
            RuleFor(p => p.Message).NotEmpty();
            RuleFor(p => p.Message).MaximumLength(5000);
            RuleFor(p => p.SecondsToLive).GreaterThan(0);
            RuleFor(p => p.SecondsToLive).LessThanOrEqualTo(60 * 60 * 24 * MaxNumberOfDaysAllowed);
        }
    }
}