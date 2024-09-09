using FluentValidation;
using Identity.Core.Application.Contracts.Identity;

namespace Identity.Core.Application.DTOs.Account.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterAccountDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .MaximumLength(256);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(256);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(x => x.RePassword)
                .NotEmpty()
                .Equal(x => x.Password);

            When(x => !string.IsNullOrEmpty(x.PhoneNumber), () =>
            {
                RuleFor(c => c.PhoneNumber)
                .Length(11);
            });
           
        }
    }
}
