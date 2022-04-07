using System;
using System.Collections.Generic;

namespace BmeModels.DbModels
{
    public partial class CategoryEntity
    {
        public CategoryEntity()
        {
            Subcategories = new HashSet<SubcategoryEntity>();
            Transactions = new HashSet<TransactionEntity>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Decription { get; set; }

        public virtual ICollection<SubcategoryEntity> Subcategories { get; set; }
        public virtual ICollection<TransactionEntity> Transactions { get; set; }
    }
}
