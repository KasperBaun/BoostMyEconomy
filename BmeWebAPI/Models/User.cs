using System;
using System.Collections.Generic;

namespace BmeWebAPI.Models
{
    public partial class User
    {
        public User()
        {
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string CreatedAt { get; set; } = null!;
        public int? Age { get; set; }
        public string? Gender { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
