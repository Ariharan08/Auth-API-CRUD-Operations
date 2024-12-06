using Auth.Models;
using Auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private static List<User> _users = new List<User>();
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (_users.Any(u => u.Username == user.Username))
                return BadRequest("User already exists.");

            _users.Add(user);
            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            var existingUser = _users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);
            if (existingUser == null)
                return Unauthorized("Invalid credentials.");

            var token = _tokenService.GenerateToken(existingUser);
            return Ok(new { Token = token });
        }
    }
}
