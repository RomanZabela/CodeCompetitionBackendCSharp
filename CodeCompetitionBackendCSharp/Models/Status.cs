using System;
using System.Collections.Generic;

namespace CodeCompetitionBackendCSharp.Models
{
    public partial class Status
    {
        public Status()
        {
            CompetitionStatuses = new HashSet<CompetitionStatus>();
            Competitions = new HashSet<Competition>();
            Participants = new HashSet<Participant>();
            Roles = new HashSet<Role>();
            TaskCategories = new HashSet<TaskCategory>();
            TaskStatuses = new HashSet<TaskStatus>();
            TaskToCompetitions = new HashSet<TaskToCompetition>();
            TaskToTeams = new HashSet<TaskToTeam>();
            Tasks = new HashSet<Task>();
            Teams = new HashSet<Team>();
            UserToRoles = new HashSet<UserToRole>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;

        public virtual ICollection<CompetitionStatus> CompetitionStatuses { get; set; }
        public virtual ICollection<Competition> Competitions { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<TaskCategory> TaskCategories { get; set; }
        public virtual ICollection<TaskStatus> TaskStatuses { get; set; }
        public virtual ICollection<TaskToCompetition> TaskToCompetitions { get; set; }
        public virtual ICollection<TaskToTeam> TaskToTeams { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
        public virtual ICollection<UserToRole> UserToRoles { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
