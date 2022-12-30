using System;
using System.Collections.Generic;

namespace CodeCompetitionBackendCSharp.Models
{
    public partial class TaskToCompetition
    {
        public int Id { get; set; }
        public int CompetitionId { get; set; }
        public int TaskId { get; set; }
        public int StatusId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int CreateUserId { get; set; }
        public int UpdateUserId { get; set; }

        public virtual Competition Competition { get; set; } = null!;
        public virtual User CreateUser { get; set; } = null!;
        public virtual Status Status { get; set; } = null!;
        public virtual Task Task { get; set; } = null!;
        public virtual User UpdateUser { get; set; } = null!;
    }
}
