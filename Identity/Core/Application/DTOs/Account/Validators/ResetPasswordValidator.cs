using FluentValidation;

namespace Identity.Core.Application.DTOs.Account.Validators
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(x => x.ReNewPassword)
               .NotEmpty()
               .Equal(x => x.NewPassword);
        }
    }
}
