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
            return _context.Teams.ToList();
        }

        public bool IsTeamExists(int teamId)
        {
            return _context.Teams.Any(t => t.Id == teamId);
        }

        public Team GetTeam (int teamId) 
        {
            return _context.Teams.Where(t => t.Id == teamId).FirstOrDefault();
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
    }
}
