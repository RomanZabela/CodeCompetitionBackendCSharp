using CodeCompetitionBackendCSharp.Models;

namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITeamService
    {
        Task<ICollection<Team>> GetTeams();
        Task<bool> TeamExists(int teamId);  
        Task<Team> GetTeam(int teamId);
        Task<Team> GetTeamByParticipantId(int participantId);
        Task<bool> CreateTeam(Team team);
        Task<bool> UpdateTeam(Team team);
        Task<bool> DeleteTeam(Team team, Enum.Status newStatus, DateTime updateDate, int updateUserId);
        Task<bool> Save();

    }
}
