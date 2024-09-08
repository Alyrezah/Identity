using FluentValidation;
using Identity.Core.Application.Contracts.Persistence;

namespace Identity.Core.Application.DTOs.Product.Validators
{
    public class CreateProductValidator : AbstractValidator<CreateProductDto>
    {
        private readonly IProductCategoryRepository _productCategoryRepository;

        public CreateProductValidator(IProductCategoryRepository productCategoryRepository)
        {
            _productCategoryRepository = productCategoryRepository;

            Include(new ProductValidatorBase());

            RuleFor(x => x.CategoryId)
                .NotNull()
                .GreaterThan(0)
                .MustAsync(async (id, token) =>
                {
                    var productCategoryIsExist = await _productCategoryRepository.IsExist(c => c.Id == id);
                    return productCategoryIsExist;
                }).WithMessage("There is no category in the list");
        }
    }
}
