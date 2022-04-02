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
        public async Task<ActionResult<IEnumerable<BmeModels.Transaction>>> GetTransactions()
        {
            List<Models.Transaction> dbTransactions = _context.Transactions.ToList();
            List<BmeModels.Transaction> transactions = new();
            foreach (var transaction in dbTransactions)
            {
                BmeModels.Transaction transactionModel = dbToModel(transaction);
                transactions.Add(transactionModel);
            }
            return transactions;
        }

        // POST: api/Transaction/
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<ActionResult<BmeModels.Transaction>> CreateTransaction(TransactionDTO transaction)
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
                MadeAt = transaction.MadeAt.ToString(),
                CategoryId = transaction.CategoryId,
                Type = transaction.Type,
                SubcategoryId = transaction.SubcategoryId,
                Description = transaction.Description,
            };

                _context.Transactions.Add(dbTransaction);
            try
            {
                await _context.SaveChangesAsync();
                return dbToModel(dbTransaction);
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

        private BmeModels.Transaction dbToModel(Models.Transaction dbTransaction)
        {
            Models.Category dbCategory = _context.Categories.Single(c => c.Id == dbTransaction.CategoryId);
            Models.Subcategory? dbSubcategory = new();
            if (_context.Subcategories.Any())
            {
                dbSubcategory = _context.Subcategories.SingleOrDefault(c => c.Id == dbTransaction.SubcategoryId);
            }
            BmeModels.Subcategory subcategory = new();
            BmeModels.Category category = new()
            {
                Id = dbCategory.Id,
                Decription = dbCategory.Decription,
                Title = dbCategory.Title,
            };
            if (dbSubcategory != null)
            {
                subcategory.Id = dbSubcategory.Id;
                subcategory.Title = dbSubcategory.Title;
                subcategory.ParentCategoryId = dbSubcategory.ParentCategoryId;
                subcategory.Description = dbSubcategory.Description;
            }
            BmeModels.Transaction transactionModel = new()
            {
                Id = dbTransaction.Id,
                UserId = dbTransaction.UserId,
                MadeAt = DateTime.Parse(dbTransaction.MadeAt),
                Source = dbTransaction.Source,
                Type = dbTransaction.Type,
                Value = dbTransaction.Value,
                Description = dbTransaction.Description,
                Category = category,
                Subcategory = subcategory,
            };
            return transactionModel;
        }
    }
}
