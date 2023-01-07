using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Paging;
using CodeCompetitionBackendCSharp.RequestFeatures;

namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITaskService
    {
        Task<PagedList<Models.Task>> GetTasks(TaskParameters taskParameters);
		Task<PagedList<TaskWithCategoryNameDto>> GetTasksExtended(TaskParameters taskParameters);

		Task<Models.Task> GetTaskById(int id);
        Task<Models.Task> GetTaskByName(string name);
        Task<ICollection<Models.Task>> GetTasksByTaskCategory(int taskCategoryId);
        Task<bool> IsTaskExist(int id);
        Task<bool> AddTask(Models.Task task);
        Task<bool> UpdateTask(Models.Task task);
        Task<bool> DeleteTask(Models.Task task);
        Task<bool> Save();
        Task<int> GetCompetitionStatusFromNodeJS(int competitionId);
    }
}
