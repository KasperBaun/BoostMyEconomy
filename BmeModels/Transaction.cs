
namespace BmeModels
{
    public class Transaction
    {
        public Transaction(){}

        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime MadeAt { get; set; } 
        public double Value { get; set; }
        public string Type { get; set; } = null!;
        public Category Category { get; set; } = new();
        public string Source { get; set; } = null!;
        public Subcategory? Subcategory { get; set; }
        public string? Description { get; set; }

        public override string ToString()
        {
            string subcategoryTitle;
            if(Subcategory == null)
            {
                subcategoryTitle = "";
            }
            else
            {
                subcategoryTitle = Subcategory.Title;
            }
            return "Id: " + Id + " " +
                    "UserId: " + UserId + " " +
                    "MadeAt: " + MadeAt.ToString() + " " +
                    "Value: " + Value.ToString() + " " +
                    "Categorytitle: " + Category.Title + " " +
                    "Source: " + Source + " " +
                    "Subcategorytitle: " + subcategoryTitle + " " +
                    "Description: " + Description;
        }

    }
}
