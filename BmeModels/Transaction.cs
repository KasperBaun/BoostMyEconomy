﻿
namespace BmeModels
{
    public class Transaction
    {
        public Transaction(){}

        public int Id { get; set; }
        public int UserId { get; set; }
        public string MadeAt { get; set; } = null!;
        public int Value { get; set; }
        public string Type { get; set; } = null!;
        public int CategoryId { get; set; }
        public string Source { get; set; } = null!;
        public int? SubcategoryId { get; set; }
        public string? Description { get; set; }
    }
}