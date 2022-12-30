namespace CodeCompetitionBackendCSharp.Dto
{
    public class TaskCategoryDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? CategoryDescription { get; set; }
    }
}
