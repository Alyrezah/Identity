using FluentValidation;
using Identity.Core.Application.DTOs.ProductCategory.Interfaces;

namespace Identity.Core.Application.DTOs.ProductCategory.Validators
{
    public class ProductCategoryValidatorBase : AbstractValidator<IProductCategoryDto>
    {
        public ProductCategoryValidatorBase()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}
