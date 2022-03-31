using BmeWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BmeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoriesController : ControllerBase
    {
        private readonly BmeDbContext _context;

        public SubCategoriesController(BmeDbContext context)
        {
            _context = context;
        }

        // GET: api/SubCategories
        [Authorize(Roles = "Admin,User")]
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<Subcategory>>> GetSubCategories()
        {
            return await _context.Subcategories.ToListAsync();
        }

        // GET: api/Categories/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Subcategory>> GetSubCategory(int subcategoryId)
        {
            var subcategory = await _context.Subcategories.FindAsync(subcategoryId);

            if (subcategory == null)
            {
                return NotFound();
            }

            return subcategory;
        }
        // POST api/<SubCategoriesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/SubCategories/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int subcategoryId, Subcategory subcategory)
        {
            if (subcategoryId != subcategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(subcategoryId).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubCategoryIdExists(subcategoryId))
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

        // DELETE: api/SubCategories/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(int subcategoryId)
        {
            var subcategory = await _context.Categories.FindAsync(subcategoryId);
            if (subcategory == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(subcategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubCategoryIdExists(int subcategoryId)
        {
            return _context.Subcategories.Any(e => e.Id == subcategoryId);
        }
    }
}
