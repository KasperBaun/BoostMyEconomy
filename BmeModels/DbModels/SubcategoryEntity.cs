using System;
using System.Collections.Generic;

namespace BmeModels.DbModels
{
    public partial class SubcategoryEntity
    {
        public int Id { get; set; }
        public int ParentCategoryId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public virtual CategoryEntity ParentCategory { get; set; } = null!;
    }
}
