using Identity.Core.Domain;

namespace Identity.Core.Application.Contracts.Persistence
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<List<Product>> GetProductList();
        Task<Product> GetProductDetail(int id);
    }
}
