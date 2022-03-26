#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BmeWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            int requestUserId = (await _context.Users.FindAsync(ClaimTypes.NameIdentifier)).Id;
            List<Transaction> transactions = _context.Transactions.Where(t => t.UserId == requestUserId).ToList();
            return transactions;
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
