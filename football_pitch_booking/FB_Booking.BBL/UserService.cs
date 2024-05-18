using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FB_Booking.DAL;
using FB_Booking.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FB_Booking.BBL
{
    public class UserService
    {
        private readonly FootballPitchBookingContext _dbContext;
        private readonly IConfiguration _configuration;


        public UserService(FootballPitchBookingContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<bool> CreateUserAsync(string username, string password, string email)
        {
            if (await _dbContext.Users.AnyAsync(u => u.UserName == username || u.Email == email))
            {
                Console.WriteLine("Validate User!");
                return false; // Username or email already exists
            }

            var user = new User
            {
                UserName = username,
                Email = email,
                PasswordHash = password,
                RoleId = 2
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine("create User successfully!");
            return true;
        }

        public async Task<string> ValidateUserAsync(string username, string password)
        {
            var user = await _dbContext.Users
                .Where(u => u.UserName == username)
                .FirstOrDefaultAsync();

            if (user == null || user.PasswordHash != password)
            {
                return null; // Invalid username or password
            }

            return GenerateJwtToken(user.UserName, user.UserId, (int)user.RoleId); // Return the JWT token
        }
        private string GenerateJwtToken(string username, int userID, int roleId)
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                SecurityAlgorithms.HmacSha256
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim("UserId", userID.ToString()),
                    new Claim("RoleId", roleId.ToString())
                }),
                Issuer = issuer,
                Audience = audience,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}
