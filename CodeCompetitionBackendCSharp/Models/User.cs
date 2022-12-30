using System;
using System.Collections.Generic;

namespace CodeCompetitionBackendCSharp.Models
{
    public partial class User
    {
        public User()
        {
            CompetitionCompetitionAdministrators = new HashSet<Competition>();
            CompetitionCreateUsers = new HashSet<Competition>();
            CompetitionStatusCreateUsers = new HashSet<CompetitionStatus>();
            CompetitionStatusUpdateUsers = new HashSet<CompetitionStatus>();
            CompetitionUpdateUsers = new HashSet<Competition>();
            ParticipantCreateUsers = new HashSet<Participant>();
            ParticipantUpdateUsers = new HashSet<Participant>();
            ParticipantUsers = new HashSet<Participant>();
            Roles = new HashSet<Role>();
            TaskCategoryCreateUsers = new HashSet<TaskCategory>();
            TaskCategoryUpdateUsers = new HashSet<TaskCategory>();
            TaskCreateUsers = new HashSet<Task>();
            TaskStatusCreateUsers = new HashSet<TaskStatus>();
            TaskStatusUpdateUsers = new HashSet<TaskStatus>();
            TaskToCompetitionCreateUsers = new HashSet<TaskToCompetition>();
            TaskToCompetitionUpdateUsers = new HashSet<TaskToCompetition>();
            TaskToTeamCreateUsers = new HashSet<TaskToTeam>();
            TaskToTeamUpdateUsers = new HashSet<TaskToTeam>();
            TaskUpdateUsers = new HashSet<Task>();
            TeamCreateUsers = new HashSet<Team>();
            TeamUpdateUsers = new HashSet<Team>();
            UserToRoleCreateUsers = new HashSet<UserToRole>();
            UserToRoleUpdateUsers = new HashSet<UserToRole>();
            UserToRoleUsers = new HashSet<UserToRole>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int CreateUserId { get; set; }
        public int UpdateUserId { get; set; }
        public int StatusId { get; set; }

        public virtual Status Status { get; set; } = null!;
        public virtual ICollection<Competition> CompetitionCompetitionAdministrators { get; set; }
        public virtual ICollection<Competition> CompetitionCreateUsers { get; set; }
        public virtual ICollection<CompetitionStatus> CompetitionStatusCreateUsers { get; set; }
        public virtual ICollection<CompetitionStatus> CompetitionStatusUpdateUsers { get; set; }
        public virtual ICollection<Competition> CompetitionUpdateUsers { get; set; }
        public virtual ICollection<Participant> ParticipantCreateUsers { get; set; }
        public virtual ICollection<Participant> ParticipantUpdateUsers { get; set; }
        public virtual ICollection<Participant> ParticipantUsers { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<TaskCategory> TaskCategoryCreateUsers { get; set; }
        public virtual ICollection<TaskCategory> TaskCategoryUpdateUsers { get; set; }
        public virtual ICollection<Task> TaskCreateUsers { get; set; }
        public virtual ICollection<TaskStatus> TaskStatusCreateUsers { get; set; }
        public virtual ICollection<TaskStatus> TaskStatusUpdateUsers { get; set; }
        public virtual ICollection<TaskToCompetition> TaskToCompetitionCreateUsers { get; set; }
        public virtual ICollection<TaskToCompetition> TaskToCompetitionUpdateUsers { get; set; }
        public virtual ICollection<TaskToTeam> TaskToTeamCreateUsers { get; set; }
        public virtual ICollection<TaskToTeam> TaskToTeamUpdateUsers { get; set; }
        public virtual ICollection<Task> TaskUpdateUsers { get; set; }
        public virtual ICollection<Team> TeamCreateUsers { get; set; }
        public virtual ICollection<Team> TeamUpdateUsers { get; set; }
        public virtual ICollection<UserToRole> UserToRoleCreateUsers { get; set; }
        public virtual ICollection<UserToRole> UserToRoleUpdateUsers { get; set; }
        public virtual ICollection<UserToRole> UserToRoleUsers { get; set; }
    }
}
