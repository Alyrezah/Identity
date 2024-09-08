using Identity.Core.Application.Contracts.Persistence;
using Identity.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repository
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly DatabaseContext _context;
        public ProductRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Product> GetProductDetail(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Products
                   .Include(x => x.Category)
                   .SingleOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<List<Product>> GetProductList()
        {
            return await _context.Products
                .Include(x=>x.Category)
                .OrderByDescending(x=>x.Id)
                .ToListAsync();
        }
    }
}
