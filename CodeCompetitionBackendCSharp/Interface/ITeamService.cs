using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.Paging;
using CodeCompetitionBackendCSharp.RequestFeatures;

namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITeamService
    {
        Task<PagedList<TeamDto>> GetTeams(TeamParameters teamsParameters);
        Task<PagedList<TeamExtendedDto>> GetTeamsExtended(TeamParameters teamsParameters);
        Task<PagedList<TeamExtendedDto>> GetTeamsExtendedByCompetitionId(TeamParameters teamsParameters, int competitionId);
		Task<bool> TeamExists(int teamId);  
        Task<Team> GetTeam(int teamId);
        Task<Team> GetTeamByParticipantId(int participantId);
        Task<bool> CreateTeam(Team team);
        Task<bool> UpdateTeam(Team team);
        Task<bool> DeleteTeam(Team team, Enum.Status newStatus, DateTime updateDate, int updateUserId);
        Task<bool> Save();

    }
}
