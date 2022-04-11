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
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (userId == 0)
            {
                return BadRequest("UserId not found!");
            }
            return await Task.Run(() =>
                {
                    List<TransactionEntity> dbTransactions = _context.Transactions.Where(t => t.UserId == userId).ToList();
                    List<Transaction> transactions = new();
                    foreach (var transaction in dbTransactions)
                    {
                        Transaction transactionModel = DbToModel(transaction);
                        transactions.Add(transactionModel);
                    }
                    return transactions;
                });
        }

        // POST: api/Transaction/
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction(TransactionDTO transaction)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return BadRequest("UserId not found!");
            }
            int id = _context.Transactions.Count() + 1;
            bool exists = _context.Transactions.Any(t => t.Id == id);
            while (exists)
            {
                id = id++;
                exists = _context.Transactions.Any(t => t.Id == id);
            }
            TransactionEntity dbTransaction = new()
            {   
                Id = id,
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
                return DbToModel(dbTransaction);
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
            TransactionEntity dbTransaction = ModelToDb(transaction);
            _context.Transactions.Update(dbTransaction);
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
        [HttpDelete]
        public async Task<IActionResult> DeleteTransaction(Transaction transaction)
        {
            var dbTransaction = await _context.Transactions.FindAsync(transaction.Id);
            if (dbTransaction == null) { return NotFound(); }

            _context.Transactions.Remove(dbTransaction);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private Transaction DbToModel(TransactionEntity dbTransaction)
        {
            CategoryEntity dbCategory = _context.Categories.Single(c => c.Id == dbTransaction.CategoryId);
            SubcategoryEntity dbSubcategory = new()
            {
                Id = 0,
                ParentCategoryId = 0,
                Title = " ",
                Description = " ",
            };
            if (_context.Subcategories.Any())
            {
                SubcategoryEntity dbSub = _context.Subcategories.Single(s => s.Id == dbTransaction.SubcategoryId);
                if(dbSub == null)
                {

                }
                else
                {
                    dbSubcategory = dbSub;
                }
            }
            Subcategory subcategory = new();
            Category category = new()
            {
                Id = dbCategory.Id,
                Decription = dbCategory.Decription,
                Title = dbCategory.Title,
            };
            
            subcategory.Id = dbSubcategory.Id;
            subcategory.Title = dbSubcategory.Title;
            subcategory.ParentCategoryId = dbSubcategory.ParentCategoryId;
            subcategory.Description = dbSubcategory.Description;
            Transaction transactionModel = new()
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

        [ApiExplorerSettings(IgnoreApi = true)]
        private static TransactionEntity ModelToDb(Transaction transaction)
        {
            string descrip = "";
            Subcategory subcategory = new();
            TransactionEntity transactionEntity = new();
            if(transaction.Category == null)
            {
                throw new Exception(message:"Category is null @ ModelToDb - TransactionController.cs ");
            }
            if(transaction.Subcategory == null)
            {
                subcategory.Id = 0;
            }
            if(!string.IsNullOrEmpty(transaction.Description))
            {
                descrip = transaction.Description;
            }

            transactionEntity.Id = transaction.Id;
            transactionEntity.UserId = transaction.UserId;
            transactionEntity.MadeAt = transaction.MadeAt.ToString();
            transactionEntity.Value = transaction.Value;
            transactionEntity.Type = transaction.Type;
            transactionEntity.CategoryId = transaction.Category.Id;
            transactionEntity.Source = transaction.Source;
            transactionEntity.SubcategoryId = subcategory.Id;
            transactionEntity.Description = descrip;

            return transactionEntity;

        }
    }
}
