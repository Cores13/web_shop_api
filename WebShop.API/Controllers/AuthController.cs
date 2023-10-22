using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using WebShop.Domain;
using WebShop.Domain.DTOs;
using WebShop.Domain.Entities;
using System.Security.Principal;
using WebShop.Application;
using WebShop.Application.Interfaces;
using WebShop.Infrastructure;

namespace WebShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public AuthController(IConfiguration configuration, ApplicationDbContext context, IAuthService authService, IEmailService emailService)
        {
            _configuration = configuration;
            _context = context;
            _authService = authService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(UserRegisterDto request)
        {
            if (request.Password != request.PasswordConfirm)
            {
                return BadRequest(new
                {
                    type = "Password",
                    message = "Password and Password confirm do not match."
                });
            }

            if (await _authService.CheckEmailExistance(request.Email))
            {
                return BadRequest(new
                {
                    type = "Email",
                    message = "Email already exists"
                });
            }

            var passwordValidation = _authService.CheckPasswordStrength(request.Password);
            if (!string.IsNullOrEmpty(passwordValidation))
            {
                return BadRequest(new
                {
                    type = "Password",
                    message = passwordValidation
                });
            }

            _authService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            User user = new User
                (
                    id: Guid.NewGuid(),
                    name: request.Name,
                    email: request.Email,
                    username: request.Username,
                    passwordHash: passwordHash,
                    passwordSalt: passwordSalt,
                    role: 1
                );

            var registeredUser = _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User created successfully"
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto request)
        {
            var user = await _context.Users.FirstAsync(u => u.Email == request.Email);

            if (user == null)
            {
                user = await _context.Users.FirstAsync(u => u.Username == request.Email);
                if (user == null)
                {
                    return BadRequest("User not found");
                }
            }
            if (!_authService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password");
            }

            string jwtToken = _authService.CreateToken(user);
            var newAccessToken = jwtToken;
            var newRefreshToken = _authService.CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(5);
            await _context.SaveChangesAsync();

            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("renewrefreshtoken")]
        public async Task<IActionResult> RenewRefreshToken(TokenApiDto tokenApiDto)
        {
            if (tokenApiDto is null)
            {
                return BadRequest("Invalid request");
            }
            string accessToken = tokenApiDto.AccessToken;
            string refreshToken = tokenApiDto.RefreshToken;

            var principal = _authService.GetPrincipleFromExpiredToken(accessToken);
            var Username = principal.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == Username);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid request");
            }
            var newAccessToken = _authService.CreateToken(user);
            var newRefreshToken = _authService.CreateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _context.SaveChangesAsync();

            return Ok(new TokenApiDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("resetpassword/{email}")]
        public async Task<IActionResult> SendResetPasswordEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if(user is null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Email doesn't exist"
                });
            }

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);

            user.ResetPasswordToken = emailToken;
            user.ResetPasswordExpiry = DateTime.Now.AddMinutes(15);

            string from = _configuration.GetSection("EmailSettings:From").Value;
            var emailModel = new Email(email, "Password reset", _emailService.CreateEmailBody(email, emailToken));

            _emailService.SendEmail(emailModel);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                StatusCode = 200,
                Message = "Email sent sucesfully"
            });
        }

        [HttpPost("password-reset")]
        public async Task<IActionResult> ResetPassword(PasswordResetDto passwordResetDto)
        {
            var newToken = passwordResetDto.EmailToken.Replace(" ", "+");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == passwordResetDto.Email);

            if(user is null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "User doesn't exist"
                });
            }

            var tokenCode = user.ResetPasswordToken;
            DateTime emailTokenExpiry = user.ResetPasswordExpiry;

            if (tokenCode != passwordResetDto.EmailToken || emailTokenExpiry < DateTime.Now)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid reset link"
                });
            }

            if (passwordResetDto.NewPassword != passwordResetDto.PasswordConfirm)
            {
                return BadRequest(new
                {
                    type = "Password",
                    message = "Password and Password confirm do not match."
                });
            }

            var passwordValidation = _authService.CheckPasswordStrength(passwordResetDto.NewPassword);
            if (!string.IsNullOrEmpty(passwordValidation))
            {
                return BadRequest(new
                {
                    type = "Password",
                    message = passwordValidation
                });
            }

            _authService.CreatePasswordHash(passwordResetDto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                StatusCode = 200,
                Message = "Password reset successful"
            });
        }
    }
}
