namespace CodeCompetitionBackendCSharp.Dto
{
    public class TaskToTeamDto
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int TaskId { get; set; }
        public int TaskStatusId { get; set; }
        public int ParticipantIdForTask { get; set; }
        public int? ReachedScore { get; set; }
    }
}
