using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BmeWebAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace BmeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BmeDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(BmeDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: api/Users/All
        [Authorize(Roles ="Admin")]
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/User/LoggedIn
        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserEntity>> GetUser(int id)
        {
            var response = await _context.Users.FindAsync(id);
            if(response != null)
            {
                BmeModels.User userObject = new()
                {
                    FirstName = response.FirstName,
                    LastName = response.LastName,
                    Email = response.Email,
                    Age = response.Age,
                    CreatedAt = response.CreatedAt,
                    Gender = response.Gender,
                    Id = response.Id,
                    RoleId = response.RoleId,
                    Password = ""
                };

                return Ok(userObject);
            }
            else
            {
                return BadRequest("Something went wrong");
            }
        }


        // PUT: api/Users/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserEntity user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserIdExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/User/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool UserIdExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
