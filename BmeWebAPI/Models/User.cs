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
        public sbyte RoleId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public sbyte? EmailVerified { get; set; }
        public string PasswordHash { get; set; } = null!;
        public int? Age { get; set; }
        public DateOnly CreatedAt { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
