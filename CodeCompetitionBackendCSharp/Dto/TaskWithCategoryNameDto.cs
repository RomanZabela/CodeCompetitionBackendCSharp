namespace CodeCompetitionBackendCSharp.Dto
{
	public class TaskWithCategoryNameDto
	{
		public int Id { get; set; }
		public string TaskName { get; set; } = null!;
		public string TaskDescription { get; set; } = null!;
		public string TaskContent { get; set; } = null!;
		public TimeSpan Timeframe { get; set; }
		public int Points { get; set; }
		public string? TaskCategoryName { get; set; }
	}
}
