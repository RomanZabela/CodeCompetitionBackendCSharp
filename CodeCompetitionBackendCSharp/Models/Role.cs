using System;
using System.Collections.Generic;

namespace CodeCompetitionBackendCSharp.Models
{
    public partial class Role
    {
        public Role()
        {
            UserToRoles = new HashSet<UserToRole>();
        }

        public int Id { get; set; }
        public string RoleName { get; set; } = null!;
        public int StatusId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int CreateUserId { get; set; }
        public int UpdateUserId { get; set; }

        public virtual Status Status { get; set; } = null!;
        public virtual User UpdateUser { get; set; } = null!;
        public virtual ICollection<UserToRole> UserToRoles { get; set; }
    }
}
