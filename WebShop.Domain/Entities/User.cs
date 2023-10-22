using WebShop.Domain.Primitives;

namespace WebShop.Domain.Entities
{
    public sealed class User : Entity
    {
        public User(Guid id, string name, string email, string username, byte[] passwordHash, byte[] passwordSalt, int role) : base(id)
        {
            Name = name;
            Email = email;
            Username = username;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            Role = role;
        }


        public string Name { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string? RefreshToken { get; set; } = string.Empty;

        public DateTime RefreshTokenExpiryTime { get; set; }

        public string? ResetPasswordToken { get; set; } = string.Empty;

        public DateTime ResetPasswordExpiry { get; set; }

        public int Role { get; set; }

    }
}
