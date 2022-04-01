using System;
using System.Collections.Generic;

namespace BmeWebAPI.Models
{
    public partial class Transaction
    {
        public Transaction()
        {
            Transactionitems = new HashSet<Transactionitem>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string MadeAt { get; set; } = null!;
        public int Value { get; set; }
        public string Type { get; set; } = null!;
        public int CategoryId { get; set; }
        public string Source { get; set; } = null!;
        public int? SubcategoryId { get; set; }
        public string? Description { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Transactionitem> Transactionitems { get; set; }
    }
}
