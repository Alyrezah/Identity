using Identity.Core.Application.DTOs.ProductCategory;

namespace Identity.Core.Application.Contracts
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategoryDto>> GetList();
        Task<ProductCategoryDto> GetBy(int id);
        Task<CommandResponse> Create(CreateProductCategoryDto command);
        Task<UpdateProductCategoryDto> GetForUpdate(int id);
        Task<CommandResponse> Update(UpdateProductCategoryDto command);
    }
}
