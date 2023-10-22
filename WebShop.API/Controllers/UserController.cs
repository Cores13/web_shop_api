using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop.Domain;
using WebShop.Domain.DTOs;
using WebShop.Domain.Entities;
using WebShop.Infrastructure;

namespace WebShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        public UserController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // [Authorize(Roles = "Admin")]
        [Authorize]
        [HttpGet(Name = "GetAllUsers")]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            List<UserDto> usersDto = new List<UserDto>();

            var users = await _context.Users.ToListAsync();

            foreach(User user in users)
            {
                var userDto = (UserDto)user;
                usersDto.Add(userDto);
            }
            return Ok(usersDto);
        }
    }
}
