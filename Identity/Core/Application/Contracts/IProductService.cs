using Identity.Core.Application.DTOs.Product;

namespace Identity.Core.Application.Contracts
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetList();
        Task<ProductDto> GetBy(int id);
        Task<CommandResponse> Create(CreateProductDto command);
        Task<UpdateProductDto> GetForUpdate(int id);
        Task<CommandResponse> Update(UpdateProductDto command);
    }
}
