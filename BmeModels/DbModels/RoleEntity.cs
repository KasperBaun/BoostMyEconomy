using System;
using System.Collections.Generic;

namespace BmeModels.DbModels
{
    public partial class RoleEntity
    {
        public RoleEntity()
        {
            Users = new HashSet<UserEntity>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<UserEntity> Users { get; set; }
    }
}
