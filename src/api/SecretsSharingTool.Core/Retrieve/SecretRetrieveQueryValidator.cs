using FluentValidation;

namespace SecretsSharingTool.Core.Retrieve
{
    public sealed class SecretRetrieveQueryValidator : AbstractValidator<SecretRetrieveQuery>
    {
        public SecretRetrieveQueryValidator()
        {
            RuleFor(p => p.Id).NotEmpty();
            RuleFor(p => p.PrivateKey).NotEmpty();
        }
    }
}