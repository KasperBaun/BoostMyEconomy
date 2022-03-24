
namespace BmeModels
{
    public class TransactionItem
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string Name { get; set; } = null!;
        public int Value { get; set; }
        public int Quantity { get; set; }
    }
}
