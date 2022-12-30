using System;
using System.Collections.Generic;

namespace CodeCompetitionBackendCSharp.Models
{
    public partial class Competition
    {
        public Competition()
        {
            TaskToCompetitions = new HashSet<TaskToCompetition>();
            Teams = new HashSet<Team>();
        }

        public int Id { get; set; }
        public string CompetitionName { get; set; } = null!;
        public int CompetitionAdministratorId { get; set; }
        public int MaxTasksPerGroups { get; set; }
        public int CompetitionStatusId { get; set; }
        public string? HashCode { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int CreateUserId { get; set; }
        public int UpdateUserId { get; set; }
        public int StatusId { get; set; }

        public virtual User CompetitionAdministrator { get; set; } = null!;
        public virtual CompetitionStatus CompetitionStatus { get; set; } = null!;
        public virtual User CreateUser { get; set; } = null!;
        public virtual Status Status { get; set; } = null!;
        public virtual User UpdateUser { get; set; } = null!;
        public virtual ICollection<TaskToCompetition> TaskToCompetitions { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
    }
}
