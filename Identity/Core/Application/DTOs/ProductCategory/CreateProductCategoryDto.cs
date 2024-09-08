using Identity.Core.Application.DTOs.ProductCategory.Interfaces;

namespace Identity.Core.Application.DTOs.ProductCategory
{
    public class CreateProductCategoryDto : IProductCategoryDto
    {
        public string Name { get; set; }
    }
}
