using System;
using System.Collections.Generic;

namespace BmeWebAPI.Models
{
    public partial class TransactionitemEntity
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string Name { get; set; } = null!;
        public double Value { get; set; }
        public int Quantity { get; set; }

        public virtual TransactionEntity Transaction { get; set; } = null!;
    }
}
