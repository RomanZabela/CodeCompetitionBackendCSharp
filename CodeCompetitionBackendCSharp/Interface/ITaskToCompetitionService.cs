using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.Paging;
using CodeCompetitionBackendCSharp.RequestFeatures;

namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITaskToCompetitionService
    {
		// Task<PagedList<TaskWithCategoryNameDto>> GetTasksToCompetitionByCompetitionId(TaskParameters taskParameters, int competitionId);
		Task<ICollection<TaskToCompetition>> GetTaskToCompetitionByCompetitionId(int competitionId);
        Task<ICollection<TaskToCompetition>> GetTaskToCompetitionByTaskId(int taskId);
        Task<bool> AddTaskToCompetition(TaskToCompetition taskToCompetition);
        Task<bool> DeleteTaskToCompetition(ICollection<TaskToCompetition> taskToCompetition);
        Task<bool> DeleteTaskToCompetitionSingle(TaskToCompetition taskToCompetition);
        Task<bool> Save();
        Task<ICollection<TaskToCompetition>> GetTasksToCompetitions();

    }
}
