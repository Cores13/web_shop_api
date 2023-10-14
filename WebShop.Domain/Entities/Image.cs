using WebShop.Domain.Primitives;

namespace WebShop.Domain.Entities
{
    public sealed class Image : Entity
    {
        private Image(Guid id, string path, Product produict) : base(id)
        {
            Path = path;
            ProductId = produict.Id;
        }

        public Guid ProductId { get; set; }

        public string Path { get; set; }
    }
}
