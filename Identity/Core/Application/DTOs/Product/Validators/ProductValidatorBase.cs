using FluentValidation;
using Identity.Core.Application.DTOs.Product.Interfaces;

namespace Identity.Core.Application.DTOs.Product.Validators
{
    public class ProductValidatorBase : AbstractValidator<IProductDto>
    {
        public ProductValidatorBase()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Price)
                .NotNull()
                .GreaterThan(0);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(500);
        }
    }
}
