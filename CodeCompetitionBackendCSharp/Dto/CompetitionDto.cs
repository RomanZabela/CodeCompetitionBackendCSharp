namespace CodeCompetitionBackendCSharp.Dto
{
    public class CompetitionDto
    {
        public string CompetitionName { get; set; } = string.Empty;
        public int MaxTaskPerGroups { get; set; }
        public int CompetitionStatus { get; set; }
        public string CompetitionAdministratorId { get; set; } = string.Empty;

    }
}
