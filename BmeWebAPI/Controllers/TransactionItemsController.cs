#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BmeWebAPI.Models;

namespace BMEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionItemsController : ControllerBase
    {
        private readonly bmeContext _context;

        public TransactionItemsController(bmeContext context)
        {
            _context = context;
        }

        // GET: api/TransactionItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionItem>>> GetTransactionItems()
        {
            return await _context.TransactionItems.ToListAsync();
        }

        // GET: api/TransactionItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionItem>> GetTransactionItem(int id)
        {
            var transactionItem = await _context.TransactionItems.FindAsync(id);

            if (transactionItem == null)
            {
                return NotFound();
            }

            return transactionItem;
        }

        // PUT: api/TransactionItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransactionItem(int id, TransactionItem transactionItem)
        {
            if (id != transactionItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(transactionItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionItemExists(id))
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

        // POST: api/TransactionItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TransactionItem>> PostTransactionItem(TransactionItem transactionItem)
        {
            _context.TransactionItems.Add(transactionItem);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TransactionItemExists(transactionItem.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTransactionItem", new { id = transactionItem.Id }, transactionItem);
        }

        // DELETE: api/TransactionItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransactionItem(int id)
        {
            var transactionItem = await _context.TransactionItems.FindAsync(id);
            if (transactionItem == null)
            {
                return NotFound();
            }

            _context.TransactionItems.Remove(transactionItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TransactionItemExists(int id)
        {
            return _context.TransactionItems.Any(e => e.Id == id);
        }
    }
}
