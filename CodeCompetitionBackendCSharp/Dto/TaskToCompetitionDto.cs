namespace CodeCompetitionBackendCSharp.Dto
{
    public class TaskToCompetitionDto
    {
        public int Id { get; set; }
        public int CompetitionId { get; set; }
        public int TaskId { get; set; }
        public int StatusId { get; set; }
    }
}
