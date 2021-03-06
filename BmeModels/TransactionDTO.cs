using System.ComponentModel.DataAnnotations;

namespace BmeModels
{
    public class TransactionDTO
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Source is required!")]
        public string Source { get; set; } = null!;

        [Required]
        [RegularExpression(@"[-0-9][^\>A-Za-z]*", ErrorMessage = "Value must only contain numbers!")]
        public double Value { get; set; }

        [Required(ErrorMessage = "Transaction date is required!")]
        public DateTime MadeAt { get; set; }

        [Required(ErrorMessage = "Category is required!")]
        public int CategoryId { get; set; }

        public string Type { get; set; } = null!;

        public int? SubcategoryId { get; set; }

        public string? Description { get; set; }
    }
}
