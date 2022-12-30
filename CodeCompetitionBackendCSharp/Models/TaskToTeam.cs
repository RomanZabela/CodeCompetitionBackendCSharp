using System;
using System.Collections.Generic;

namespace CodeCompetitionBackendCSharp.Models
{
    public partial class TaskToTeam
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int TaskId { get; set; }
        public int TaskStatusId { get; set; }
        public int StatusId { get; set; }
        public int ParticipantIdForTask { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int CreateUserId { get; set; }
        public int UpdateUserId { get; set; }
        public int? ReachedScore { get; set; }

        public virtual User CreateUser { get; set; } = null!;
        public virtual Participant ParticipantIdForTaskNavigation { get; set; } = null!;
        public virtual Status Status { get; set; } = null!;
        public virtual Task Task { get; set; } = null!;
        public virtual TaskStatus TaskStatus { get; set; } = null!;
        public virtual Team Team { get; set; } = null!;
        public virtual User UpdateUser { get; set; } = null!;
    }
}
