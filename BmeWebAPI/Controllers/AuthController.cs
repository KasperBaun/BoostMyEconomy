using BmeModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BmeWebAPI.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static User user = new();

        private readonly BmeDbContext _context;

        public AuthController(BmeDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserRegistrationDTO newUser)
        {
            if (!UserExists(newUser.Email)) 
            {
                user.Id = _context.Users.Count() + 1;
                user.RoleId = 2;
                user.FirstName = newUser.FirstName;
                user.LastName = newUser.LastName;
                user.Email = newUser.Email;
                user.CreatedAt = DateOnly.FromDateTime(DateTime.Now).ToString();
                user.Age = null;
                user.Gender = null;
                CreatePasswordHash(newUser.Password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                _context.Users.Add(user);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (UserExists(newUser.Email))
                    {
                        return Conflict("User already exists!");
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok(user);
            }
            else
            {
                return BadRequest("User already exists!");
            }
            
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
        private bool UserExists(string email)
        {
            return _context.Users.Any(e => e.Email == email);
        }
    }
}
