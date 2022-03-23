using System;
using System.Collections.Generic;

namespace BmeWebAPI.Models
{
    public partial class Transaction
    {
        public Transaction()
        {
            TransactionItems = new HashSet<TransactionItem>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime MadeAt { get; set; }
        public int Value { get; set; }
        public string Type { get; set; } = null!;
        public int CategoryId { get; set; }
        public int? SubcategoryId { get; set; }
        public string? Description { get; set; }
        public string? Source { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<TransactionItem> TransactionItems { get; set; }
    }
}
