using Identity.Core.Application.Contracts.Persistence;
using Identity.Core.Domain;

namespace Identity.Infrastructure.Persistence.Repository
{
    public class ProductCategoryRepository : GenericRepository<ProductCategory>, IProductCategoryRepository
    {
        private readonly DatabaseContext _context;
        public ProductCategoryRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }
    }
}
