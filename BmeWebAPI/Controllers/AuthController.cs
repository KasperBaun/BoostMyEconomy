using BmeModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace BmeWebAPI.Models
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
            var exists = await _context.Users.AnyAsync(e => e.Email == email);
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
            
                var dbUser =  await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
                    // Lookup user in DB so we can compare hash and salt
                if (dbUser == null)
                {
                    return BadRequest("Something went wrong");
                }

                if (!VerifyPasswordHash(request.Password, dbUser.PasswordHash, dbUser.PasswordSalt))
                {
                    return BadRequest("Wrong password");
                }

                string token = CreateToken(dbUser);
                return Ok(token);
           
        }

        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register(UserRegistrationDTO newUser)
        {
            var userExists = await _context.Users.AnyAsync(e => e.Email == newUser.Email);
            if (!userExists) 
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

        private string CreateToken(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.FirstName+user.LastName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:VerySecretKey").Value));

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
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
      

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);  
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}
