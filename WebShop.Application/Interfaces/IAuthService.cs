using System.Security.Claims;
using WebShop.Domain.Entities;

namespace WebShop.Application.Interfaces
{
    public interface IAuthService
    {
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        string CreateToken(User user);
        Task<bool> CheckEmailExistance(string email);
        string CheckPasswordStrength(string password);
        string CreateRefreshToken();
        ClaimsPrincipal GetPrincipleFromExpiredToken(string token);
    }
}
