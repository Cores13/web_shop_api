using WebShop.Domain.Primitives;

namespace WebShop.Domain.Entities
{
    public sealed class Role : Entity
    {
        private Role(Guid id, string name, int roleId) : base(id)
        {
            Name = name;
            RoleId = roleId;
        }

        public string Name { get; set; }

        public int RoleId { get; set; }
    }
}
