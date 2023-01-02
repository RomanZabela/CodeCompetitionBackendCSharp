using CodeCompetitionBackendCSharp.Models;

namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITeamService
    {
        ICollection<Team> GetTeams();
        bool TeamExists(int teamId);  
        Team GetTeam(int teamId);
        Team GetTeamByParticipantId(int participantId);
        bool CreateTeam(Team team);
        bool UpdateTeam(Team team);
        bool DeleteTeam(Team team, Enum.Status newStatus, DateTime updateDate, int updateUserId);
        bool Save();

    }
}
