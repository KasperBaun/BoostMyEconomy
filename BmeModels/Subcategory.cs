
namespace BmeModels
{
    public class Subcategory
    {
        public int Id { get; set; }
        public int ParentCategoryId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }
}
