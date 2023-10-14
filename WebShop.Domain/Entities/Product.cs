using WebShop.Domain.Primitives;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Domain.Entities
{
    public sealed class Product : Entity
    {
        private Product(
            Guid id,
            string name,
            string description,
            decimal price,
            long inStock
            ) : base(id)
        {
            Name = name;
            Description = description;
            Price = price;
            InStock = inStock;
        }

        public string Name { get; set; }
        
        public string Description { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        public decimal Price { get; set; }

        public long InStock { get; set; }

        public ICollection<Image> Images { get; set; } = null!;
    }
}
