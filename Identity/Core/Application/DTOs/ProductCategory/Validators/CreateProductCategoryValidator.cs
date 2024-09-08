using FluentValidation;

namespace Identity.Core.Application.DTOs.ProductCategory.Validators
{
    public class CreateProductCategoryValidator : AbstractValidator<CreateProductCategoryDto>
    {
        public CreateProductCategoryValidator()
        {
            Include(new ProductCategoryValidatorBase());
        }
    }
}
