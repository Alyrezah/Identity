using Identity.Core.Application.DTOs.ProductCategory.Interfaces;

namespace Identity.Core.Application.DTOs.ProductCategory
{
    public class ProductCategoryDto : IProductCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreationDate { get; set; }
    }
}
