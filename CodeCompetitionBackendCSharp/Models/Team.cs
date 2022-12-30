using System;
using System.Collections.Generic;

namespace CodeCompetitionBackendCSharp.Models
{
    public partial class Team
    {
        public Team()
        {
            TaskToTeams = new HashSet<TaskToTeam>();
        }

        public int Id { get; set; }
        public string TeamName { get; set; } = null!;
        public int TeamLeaderId { get; set; }
        public int TotalPoints { get; set; }
        public byte[]? IconImage { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int CreateUserId { get; set; }
        public int UpdateUserId { get; set; }
        public int StatusId { get; set; }
        public int CompetitionId { get; set; }

        public virtual Competition Competition { get; set; } = null!;
        public virtual User CreateUser { get; set; } = null!;
        public virtual Status Status { get; set; } = null!;
        public virtual Participant TeamLeader { get; set; } = null!;
        public virtual User UpdateUser { get; set; } = null!;
        public virtual ICollection<TaskToTeam> TaskToTeams { get; set; }
    }
}
