namespace CodeCompetitionBackendCSharp.Dto
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string TeamName { get; set; } = null!;
        public int TeamLeaderId { get; set; }
        public int TotalPoints { get; set; }
        public int? CompetitionId { get; set; }
        public string IconImage { get; set; }
	}
}
