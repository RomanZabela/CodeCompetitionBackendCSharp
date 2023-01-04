using CodeCompetitionBackendCSharp.Models;

namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITaskToCompetitionService
    {
        Task<ICollection<TaskToCompetition>> GetTaskToCompetitionByCompetitionId(int competitionId);
        Task<ICollection<TaskToCompetition>> GetTaskToCompetitionByTaskId(int taskId);
        Task<bool> AddTaskToCompetition(TaskToCompetition taskToCompetition);
        Task<bool> DeleteTaskToCompetition(ICollection<TaskToCompetition> taskToCompetition);
        Task<bool> Save();
    }
}
