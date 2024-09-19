using FluentValidation;

namespace Identity.Core.Application.DTOs.Account.Validators
{
    public class RegisterWithExternalLoginValidator : AbstractValidator<RegisterWithExternalLoginDto>
    {
        public RegisterWithExternalLoginValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .MaximumLength(256);

            When(x => !string.IsNullOrEmpty(x.PhoneNumber), () =>
            {
                RuleFor(c => c.PhoneNumber)
                .Length(11);
            });
        }
    }
}
