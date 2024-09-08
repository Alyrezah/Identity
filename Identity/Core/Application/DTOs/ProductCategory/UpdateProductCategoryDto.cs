using Identity.Core.Application.DTOs.ProductCategory.Interfaces;

namespace Identity.Core.Application.DTOs.ProductCategory
{
    public class UpdateProductCategoryDto : IProductCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
