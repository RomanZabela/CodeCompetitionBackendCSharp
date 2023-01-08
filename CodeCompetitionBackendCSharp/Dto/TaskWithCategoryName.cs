namespace CodeCompetitionBackendCSharp.Dto
{
    public class TaskWithCategoryName
    {
        public int Id { get; set; }
        public string TaskName { get; set; } = null!;
        public string TaskDescription { get; set; } = null!;
        public string TaskContent { get; set; } = null!;
        public string TaskCategoryName { get; set; }
        public TimeSpan? Timeframe { get; set; }
        public int Points { get; set; }
    }
}
