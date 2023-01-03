using CodeCompetitionBackendCSharp.Models;

namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITaskToCompetitionService
    {
        ICollection<TaskToCompetition> GetTasksIdsByCompetitionId(int competitionId);
        ICollection<TaskToCompetition> GetCompetitionIdsByTaskId(int taskId);
        bool AddConnectionTaskIdAndCompetionId(TaskToCompetition taskToCompetition);
        bool DeleteTaskToCompetitionConnection(TaskToCompetition taskToCompetition);
        bool Save();
    }
}
