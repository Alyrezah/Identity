using FluentValidation;

namespace Identity.Core.Application.DTOs.Account.Validators
{
    public class CreateRoleValidator : AbstractValidator<CreateRoleDto>
    {
        public CreateRoleValidator()
        {
            RuleFor(x=>x.Name)
                .NotEmpty()
                .MaximumLength(256);
        }
    }
}
