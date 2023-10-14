using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestApp.Database;
using TestApp.Database.DTOs;
using TestApp.Database.Models;

namespace TestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;
        public UserController(IConfiguration configuration, DataContext context)
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
