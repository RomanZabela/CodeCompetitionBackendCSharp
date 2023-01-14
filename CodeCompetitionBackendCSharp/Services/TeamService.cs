using CodeCompetitionBackendCSharp.Data;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.Paging;
using CodeCompetitionBackendCSharp.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CodeCompetitionBackendCSharp.Services
{
    public class TeamService : ITeamService
    {
        private readonly DataContext _context;

        public TeamService(DataContext context)
        {
            _context = context;
        }
        public async Task<PagedList<Team>> GetTeams(TeamParameters teamParameters)
        {
            var result = await _context.Teams.Where(t => t.StatusId == (int)Enum.Status.Active).ToListAsync();
            return PagedList<Team>.ToPagedList(result, teamParameters.PageNumber, teamParameters.PageSize, teamParameters.switchOff);
        }

        public async Task<bool> TeamExists(int teamId)
        {
            return await _context.Teams.AnyAsync(t => t.Id == teamId);
        }

        public async Task<Team> GetTeam (int teamId) 
        {
            return await _context.Teams.Where(t => (t.Id == teamId) && (t.StatusId == (int)Enum.Status.Active)).FirstOrDefaultAsync();
        }

        public async Task<Team> GetTeamByParticipantId(int participantId)
        {
            int teamId = await _context.Participants.Where(p => p.Id == participantId).Select(p => p.TeamId).FirstOrDefaultAsync();
            return await _context.Teams.Where(t => t.Id == teamId).FirstOrDefaultAsync();
        }

        public async Task<bool> CreateTeam(Team team)
        {
            _context.Add(team);
            return await Save();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> UpdateTeam(Team team)
        {
            _context.Attach(team);

            _context.Entry(team).Property(t => t.TeamName).IsModified = true;
            _context.Entry(team).Property(t => t.TeamLeaderId).IsModified = true;
            _context.Entry(team).Property(t => t.TotalPoints).IsModified = true;
            _context.Entry(team).Property(t => t.UpdateDate).IsModified = true;
            _context.Entry(team).Property(t => t.UpdateUserId).IsModified = true;
            _context.Entry(team).Property(t => t.CompetitionId).IsModified = true;

            return await Save();
        }

        public async Task<bool> DeleteTeam(Team team, Enum.Status newStatus, DateTime updateDate, int updateUserId)
        {
            team.StatusId = (int)newStatus;
            team.UpdateDate = updateDate;
            team.UpdateUserId = updateUserId;

            _context.Attach(team);

            _context.Entry(team).Property(t => t.StatusId).IsModified = true;
            _context.Entry(team).Property(t => t.UpdateUserId).IsModified = true;
            _context.Entry(team).Property(t => t.UpdateDate).IsModified = true;

            return await Save();
        }

        public async Task<PagedList<TeamExtendedDto>> GetTeamsExtended(TeamParameters teamsParameters)
        {
            teamsParameters.switchOff = teamsParameters.switchOffString == "1" ? true : false; 
            var results = await (from t in _context.Teams.Where(t => t.StatusId == (int)Enum.Status.Active)
                                 join p in _context.Participants on t.TeamLeaderId equals p.Id
                                 join c in _context.Competitions on t.CompetitionId equals c.Id
                                 select new TeamExtendedDto
                                 {
                                     Id = t.Id,
                                     TeamName = t.TeamName,
                                     TeamLeaderEmail = p.Email,
                                     TotalPoints = t.TotalPoints,
                                     CompetitionName = c.CompetitionName
                                 }).ToListAsync();

            return PagedList<TeamExtendedDto>.ToPagedList(results, teamsParameters.PageNumber, teamsParameters.PageSize, teamsParameters.switchOff);
        }
    }
}
