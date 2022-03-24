
namespace BmeModels
{
    public class User
    {
        public User() { }

        public int Id { get; set; }
        public sbyte RoleId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public sbyte? EmailVerified { get; set; }
        public string Password { get; set; } = null!;
        public int? Age { get; set; }
        public string CreatedAt { get; set; }
    }
}
