using BmeModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using BmeWebAPI.Models;
using System.Text;

namespace BmeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly BmeDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(BmeDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("UserExists")]
        public async Task<ActionResult<bool>> UserExists(string email)
        {
            Models.User response = await _context.Users.SingleOrDefaultAsync(e => e.Email == email);
            Console.WriteLine(response.Email);
            
            if (response == null)
            {
                return BadRequest("User not found");
            }
            
            // TODO : Handle a way for the user to reset password
            return Ok(true);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(UserLoginDTO request)
        {
            Models.User dbUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            // Lookup user in DB so we can compare hash and salt
            if (dbUser == null)
            {
                return BadRequest("User not found");
            }
            if (!VerifyPasswordHash(request.Password, dbUser.PasswordHash, dbUser.PasswordSalt))
            {
                return BadRequest("Wrong password");
            }

            string token = CreateToken(dbUser).Result;
            return Ok(token);
           
        }

        [HttpPost("Register")]
        public async Task<ActionResult<Models.User>> Register(UserRegistrationDTO newUser)
        {
            var userExists = await _context.Users.AnyAsync(e => e.Email == newUser.Email);
            if (!userExists) 
            {
                Models.User user = new();
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

        private async Task<string> CreateToken(Models.User user)
        {
            Models.Role userRole = await _context.Roles.FindAsync(user.RoleId);
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.FirstName+" "+user.LastName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, userRole.Title),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
      

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}
