using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using WebShop.Domain.Entities;
using WebShop.Domain;
using WebShop.Application.Interfaces;
using WebShop.Infrastructure;

namespace WebShop.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        public AuthService(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public string CreateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var claims = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            });

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _configuration.GetSection("AppSettings:Token").Value)
                );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.Now.AddSeconds(10),
                SigningCredentials = creds
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwt = jwtTokenHandler.WriteToken(token);

            return jwt;
        }

        public Task<bool> CheckEmailExistance(string email)
        {
            return _context.Users.AnyAsync(u => u.Email == email);
        }

        public string CheckPasswordStrength(string password)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (password.Length < 8)
            {
                stringBuilder.Append("Minimum password length should be 8 characters." + Environment.NewLine);
            }
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
            {
                stringBuilder.Append("Password should be alphanumeric." + Environment.NewLine);
            }
            if (!Regex.IsMatch(password, "[`~,!,@,#,$,%,^,&,*,(,),+,=,{,},/,|,:,;,\\,\\[,\\],|,',<,>,.,?,_,]"))
            {
                stringBuilder.Append("Password should contain special characters." + Environment.NewLine);
            }
            return stringBuilder.ToString();
        }

        public string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);

            var userRefreshToken = _context.Users.Any(u => u.RefreshToken == refreshToken);
            if (userRefreshToken)
            {
                return CreateRefreshToken();
            }
            return refreshToken;
        }

        public ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
        {
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}