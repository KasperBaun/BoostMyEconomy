using System;
using System.Collections.Generic;

namespace BmeWebAPI.Models
{
    public partial class Transactionitem
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string Name { get; set; } = null!;
        public int Value { get; set; }
        public int Quantity { get; set; }

        public virtual Transaction Transaction { get; set; } = null!;
    }
}
