using FB_Booking.BBL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FB_Booking.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace football_pitch_booking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            var success = await _userService.CreateUserAsync(request.Username, request.Password, request.Email);

            if (success)
            {
                return Ok(new { message = "User created successfully" });
            }

            return BadRequest(new { message = "Username or email already exists" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _userService.ValidateUserAsync(request.Username, request.Password);

            if (token != null)
            {
                return Ok(new { token });
            }

            return Unauthorized(new { message = "Invalid username or password" });
        }
    }

    // DTO for login request
    public class SignupRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

}


