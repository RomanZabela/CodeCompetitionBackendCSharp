namespace CodeCompetitionBackendCSharp.Dto
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public string TaskContent { get; set; } = string.Empty;
        public int TaskCategoryId { get; set; }
        public TimeSpan? Timeframe { get; set; }
        public int Points { get; set; }
    }
}
