using FluentValidation;

namespace Identity.Core.Application.DTOs.ProductCategory.Validators
{
    public class UpdateProductCategoryValidator : AbstractValidator<UpdateProductCategoryDto>
    {
        public UpdateProductCategoryValidator()
        {
            Include(new ProductCategoryValidatorBase());
        }
    }
}
