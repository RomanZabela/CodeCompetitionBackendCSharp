using System;
using System.Collections.Generic;

namespace CodeCompetitionBackendCSharp.Models
{
    public partial class Participant
    {
        public Participant()
        {
            TaskToTeams = new HashSet<TaskToTeam>();
            Teams = new HashSet<Team>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int CreateUserId { get; set; }
        public int UpdateUserId { get; set; }
        public int StatusId { get; set; }

        public virtual User CreateUser { get; set; } = null!;
        public virtual Status Status { get; set; } = null!;
        public virtual User UpdateUser { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<TaskToTeam> TaskToTeams { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
    }
}
