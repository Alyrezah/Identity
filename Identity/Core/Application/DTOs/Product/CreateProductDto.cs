using Identity.Core.Application.DTOs.Product.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Identity.Core.Application.DTOs.Product
{
    public class CreateProductDto : IProductDto
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }

        public SelectList SelectCategory { get; set; }
    }
}
