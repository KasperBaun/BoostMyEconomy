using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BmeWebAPI.Models;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            List<Transaction> transactions = _context.Transactions.ToList();
            return transactions;
        }

        // POST: api/Transaction/
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> CreateTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
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
        public async Task<IActionResult> UpdateTransaction(Transaction transaction)
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
