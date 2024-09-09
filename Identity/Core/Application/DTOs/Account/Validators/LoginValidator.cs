using FluentValidation;

namespace Identity.Core.Application.DTOs.Account.Validators
{
    public class LoginValidator : AbstractValidator<LoginAccountDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
            .NotEmpty();
        }
    }
}
