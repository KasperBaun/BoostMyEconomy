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

        private readonly BmeDbContext _context;

        public AuthController(BmeDbContext context)
        {
            _context = context;
        }

        [HttpPost("UserExists")]
        public async Task<ActionResult<bool>> UserExists(string email)
        {
            bool exists = UserExistsEmail(email);
            if (! exists)
            {
                return BadRequest("User not found");
            }
            else
            {
                // TODO : Handle a way for the user to reset password
                return Ok(true);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(UserLoginDTO request)
        {
            try
            {
                User dbUser = _context.Users.FirstOrDefault(x => x.Email == request.Email);
                 // Lookup user in DB so we can compare hash and salt
                if (dbUser == null)
                {
                    return BadRequest("Something went wrong");
                }

                if (!VerifyPasswordHash(request.Password, dbUser.PasswordHash, dbUser.PasswordSalt))
                {
                    return BadRequest("Wrong password");
                }

                return Ok("My crazy token");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return BadRequest("Something went wrong");
            
        }

        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register(UserRegistrationDTO newUser)
        {
            if (!UserExistsEmail(newUser.Email)) 
            {
                User user = new();
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
                    throw;
                }

                return Ok(user);
            }
            else
            {
                return Conflict("User already exists!");
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
        private bool UserExistsEmail(string email)
        {
            return _context.Users.Any(e => e.Email == email);
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);  
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}
