namespace CodeCompetitionBackendCSharp.Dto
{
    public class TeamExtendedDto
    {
        public int Id { get; set; }
        public string TeamName { get; set; } = null!;
        public string TeamLeaderEmail { get; set; }
        public int TotalPoints { get; set; }
        public string CompetitionName { get; set; }
    }
}
