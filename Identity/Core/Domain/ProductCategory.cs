
namespace Identity.Core.Domain
{
    public class ProductCategory : BaseEntityDomain
    {
        public string Name { get; set; }
        public List<Product> Products { get; set; }

        public ProductCategory(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Products = new List<Product>();
        }

        public void Edit(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
