
namespace Identity.Core.Domain
{
    public class Product : BaseEntityDomain
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public ProductCategory Category { get; set; }

        public Product(string name, double price, string description, int categoryId)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Price = price > 0 ? price : throw new ArgumentOutOfRangeException(nameof(price));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            CategoryId = categoryId > 0 ? categoryId : throw new ArgumentOutOfRangeException(nameof(categoryId));
        }

        public void Edit(string name, double price, string description, int categoryId)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Price = price > 0 ? price : throw new ArgumentOutOfRangeException(nameof(price));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            CategoryId = categoryId > 0 ? categoryId : throw new ArgumentOutOfRangeException(nameof(categoryId));
        }
    }
}
