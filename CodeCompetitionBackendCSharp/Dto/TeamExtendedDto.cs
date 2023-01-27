namespace CodeCompetitionBackendCSharp.Dto
{
    public class TeamExtendedDto
    {
        public int Id { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string TeamLeaderEmail { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public string CompetitionName { get; set; } = string.Empty;
        public int? CompetitionId { get; set; }
        public string IconImage { get; set; } = string.Empty;

	}
}
