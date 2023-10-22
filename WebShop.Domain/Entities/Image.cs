using WebShop.Domain.Primitives;

namespace WebShop.Domain.Entities
{
    public sealed class Image : Entity
    {
        public Image(Guid id, string path, Guid productId) : base(id)
        {
            Path = path;
            ProductId = productId;
        }

        public string Path { get; set; }

        public Guid ProductId { get; set; }

        public Product Product { get; set; }
    }
}
