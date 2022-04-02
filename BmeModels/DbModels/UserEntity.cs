using System;
using System.Collections.Generic;

namespace BmeModels.DbModels
{
    public partial class UserEntity
    {
        public UserEntity()
        {
            Transactions = new HashSet<TransactionEntity>();
        }

        public int Id { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public string CreatedAt { get; set; } = null!;
        public int? Age { get; set; }
        public string? Gender { get; set; }

        public virtual RoleEntity Role { get; set; } = null!;
        public virtual ICollection<TransactionEntity> Transactions { get; set; }
    }
}
