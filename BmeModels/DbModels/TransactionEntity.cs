using System;
using System.Collections.Generic;

namespace BmeModels.DbModels
{
    public class TransactionEntity
    {
        public TransactionEntity()
        {
            Transactionitems = new HashSet<Transactionitem>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string MadeAt { get; set; } = null!;
        public double Value { get; set; }
        public string Type { get; set; } = null!;
        public int CategoryId { get; set; }
        public string Source { get; set; } = null!;
        public int? SubcategoryId { get; set; }
        public string? Description { get; set; }

        public virtual CategoryEntity Category { get; set; } = null!;
        public virtual UserEntity User { get; set; } = null!;
        public virtual ICollection<Transactionitem> Transactionitems { get; set; }
    }
}
