using System;
using System.Collections.Generic;

namespace BmeWebAPI.Models
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
