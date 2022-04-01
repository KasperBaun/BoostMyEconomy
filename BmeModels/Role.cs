
namespace BmeModels
{
    public class Role
    {
        public Role(){}
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }
}
