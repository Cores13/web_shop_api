using WebShop.Domain.Entities;

namespace WebShop.Domain.DTOs
{
    public class UserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Role { get; set; }

        public static explicit operator UserDto(User user)
        {
            return new UserDto
            {
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
