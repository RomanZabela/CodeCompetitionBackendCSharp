using CodeCompetitionBackendCSharp.Models;

namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITaskToTeamService
    {
        Task<ICollection<TaskToTeam>> GetTeamsTasks();
        Task<bool> AddTeamsTasks(TaskToTeam taskToTeam);
		Task<bool> SubmitTask(TaskToTeam taskToTeam);
		Task<bool> DeleteTeamTask(Enum.TaskStatus newTaskStatus, TaskToTeam taskToTeam);
        Task<int> GetTaskCountByTeamId(int teamId);
        Task<bool> Save();
        
    }
}
