using FluentValidation;
using Identity.Core.Application.Contracts.Persistence;

namespace Identity.Core.Application.DTOs.Product.Validators
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        public UpdateProductValidator(IProductCategoryRepository productCategoryRepository)
        {
            _productCategoryRepository = productCategoryRepository;

            Include(new ProductValidatorBase());

            RuleFor(x => x.Id)
                .NotNull()
                .GreaterThan(0);

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
