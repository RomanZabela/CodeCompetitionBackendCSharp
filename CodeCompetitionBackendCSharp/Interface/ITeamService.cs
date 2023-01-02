using CodeCompetitionBackendCSharp.Models;

namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITeamService
    {
        ICollection<Team> GetTeams();
        bool IsTeamExists(int teamId);  
        Team GetTeam(int teamId);
        Team GetTeamByParticipantId(int participantId);

    }
}
