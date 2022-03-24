using System;
using System.Collections.Generic;

namespace BmeWebAPI.Models
{
    public partial class Category
    {
        public Category()
        {
            Subcategories = new HashSet<Subcategory>();
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Decription { get; set; }

        public virtual ICollection<Subcategory> Subcategories { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
