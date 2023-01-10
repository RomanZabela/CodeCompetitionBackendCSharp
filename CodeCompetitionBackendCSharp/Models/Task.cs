using System;
using System.Collections.Generic;

namespace CodeCompetitionBackendCSharp.Models
{
    public partial class Task
    {
        public Task()
        {
            TaskToCompetitions = new HashSet<TaskToCompetition>();
            TaskToTeams = new HashSet<TaskToTeam>();
        }

        public int Id { get; set; }
        public string TaskName { get; set; } = null!;
        public string TaskDescription { get; set; } = null!;
        public string TaskContent { get; set; } = null!;
        public int TaskCategoryId { get; set; }
        public TimeSpan Timeframe { get; set; }
        public int Points { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int CreateUserId { get; set; }
        public int UpdateUserId { get; set; }
        public int StatusId { get; set; }

        public virtual User CreateUser { get; set; } = null!;
        public virtual Status Status { get; set; } = null!;
        public virtual TaskCategory TaskCategory { get; set; } = null!;
        public virtual User UpdateUser { get; set; } = null!;
        public virtual ICollection<TaskToCompetition> TaskToCompetitions { get; set; }
        public virtual ICollection<TaskToTeam> TaskToTeams { get; set; }
    }
}
