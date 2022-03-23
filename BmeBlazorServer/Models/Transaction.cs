namespace BmeBlazorServer.Models
{
    public class Transaction
    {
        public Transaction()
        {
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

    };
}
