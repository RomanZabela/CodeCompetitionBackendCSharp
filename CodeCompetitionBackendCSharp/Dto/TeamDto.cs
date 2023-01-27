namespace CodeCompetitionBackendCSharp.Dto
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public int TeamLeaderId { get; set; }
        public int TotalPoints { get; set; }
        public int? CompetitionId { get; set; }
        public string IconImage { get; set; } = string.Empty;
	}
}
