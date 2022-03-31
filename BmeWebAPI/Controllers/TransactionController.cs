using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BmeWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using BmeModels;
using System.Security.Claims;

namespace BmeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly BmeDbContext _context;

        public TransactionController(BmeDbContext context)
        {
            _context = context;
        }

        // GET: api/Transaction/All
        [Authorize(Roles ="Admin,User")]
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<Models.Transaction>>> GetTransactions()
        {
            List<Models.Transaction> transactions = _context.Transactions.ToList();
            return transactions;
        }

        // POST: api/Transaction/
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> CreateTransaction(TransactionDTO transaction)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return BadRequest("UserId not found!");
            }
            Models.Transaction dbTransaction = new()
            {   
                Id = _context.Transactions.Count() + 1,
                UserId = int.Parse(userId.Value),
                Source = transaction.Source,
                Value = transaction.Value,
                MadeAt = transaction.MadeAt,
                CategoryId = transaction.CategoryId,
                Type = transaction.Type,
                SubcategoryId = transaction.SubcategoryId,
                Description = transaction.Description,
            };

                _context.Transactions.Add(dbTransaction);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("$TransactionController_CreateTransaction(): " + ex.Message);
                return Conflict("Transaction already exists!");
            }
        }

        // PUT: api/Transaction/
        [Authorize(Roles = "Admin,User")]
        [HttpPut]
        public async Task<IActionResult> UpdateTransaction(Models.Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("$TransactionController_CreateTransaction(): " + ex.Message);
                return Conflict("Transaction update failed @ TransactionController UpdateTransaction()!");
            }
        }

        // DELETE: api/Transaction/5
        [Authorize(Roles = "Admin,User")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
