namespace CodeCompetitionBackendCSharp.Dto
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string TeamName { get; set; } = null!;
        public int TeamLeaderId { get; set; }
        public int TotalPoints { get; set; }
        public byte[]? IconImage { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int CreateUserId { get; set; }
        public int UpdateUserId { get; set; }
        public int StatusId { get; set; }
        public int CompetitionId { get; set; }
    }
}
