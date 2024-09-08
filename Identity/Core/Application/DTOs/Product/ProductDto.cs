using Identity.Core.Application.DTOs.Product.Interfaces;
using Identity.Core.Application.DTOs.ProductCategory;

namespace Identity.Core.Application.DTOs.Product
{
    public class ProductDto : IProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string CreationDate { get; set; }
        public string Description { get; set; }
        public ProductCategoryDto Category { get; set; }
    }
}
