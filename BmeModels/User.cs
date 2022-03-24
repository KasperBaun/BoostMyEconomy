
namespace BmeModels
{
    public class User
    {
        public User() { }

        public int Id { get; set; }
        public sbyte RoleId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public sbyte? EmailVerified { get; set; }
        public string PasswordHash { get; set; } = null!;
        public int? Age { get; set; }
        public DateOnly CreatedAt { get; set; }
    }
}
