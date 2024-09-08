namespace Identity.Core.Application.DTOs.Product.Interfaces
{
    public interface IProductDto
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
    }
}
