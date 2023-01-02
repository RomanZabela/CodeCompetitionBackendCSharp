using CodeCompetitionBackendCSharp.Data;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;

namespace CodeCompetitionBackendCSharp.Services
{
    public class TeamService : ITeamService
    {
        private readonly DataContext _context;

        public TeamService(DataContext context)
        {
            _context = context;
        }
        public ICollection<Team> GetTeams()
        {
            return _context.Teams.Where(t => t.StatusId == (int)Enum.Status.Active).ToList();
        }

        public bool TeamExists(int teamId)
        {
            return _context.Teams.Any(t => t.Id == teamId);
        }

        public Team GetTeam (int teamId) 
        {
            return _context.Teams.Where(t => (t.Id == teamId) && (t.StatusId == (int)Enum.Status.Active)).FirstOrDefault();
        }

        public Team GetTeamByParticipantId(int participantId)
        {
            int teamId = _context.Participants.Where(p => p.Id == participantId).Select(p => p.TeamId).FirstOrDefault();
            return _context.Teams.Where(t => t.Id == teamId).FirstOrDefault();
        }

        public bool CreateTeam(Team team)
        {
            _context.Add(team);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateTeam(Team team)
        {
            _context.Attach(team);

            _context.Entry(team).Property(t => t.TeamName).IsModified = true;
            _context.Entry(team).Property(t => t.TeamLeaderId).IsModified = true;
            _context.Entry(team).Property(t => t.TotalPoints).IsModified = true;
            _context.Entry(team).Property(t => t.UpdateDate).IsModified = true;
            _context.Entry(team).Property(t => t.UpdateUserId).IsModified = true;
            _context.Entry(team).Property(t => t.CompetitionId).IsModified = true;

            return Save();
        }

        public bool DeleteTeam(Team team, Enum.Status newStatus, DateTime updateDate, int updateUserId)
        {
            team.StatusId = (int)newStatus;
            team.UpdateDate = updateDate;
            team.UpdateUserId = updateUserId;

            _context.Attach(team);

            _context.Entry(team).Property(t => t.StatusId).IsModified = true;
            _context.Entry(team).Property(t => t.UpdateUserId).IsModified = true;
            _context.Entry(team).Property(t => t.UpdateDate).IsModified = true;

            return Save();
        }
    }
}
