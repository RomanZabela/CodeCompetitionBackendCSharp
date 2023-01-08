using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.Paging;

namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITaskService
    {
        Task<ICollection<Models.Task>> GetTasks();
        Task<PagedList<Models.Task>> GetTasksPaging(TaskParameters taskParameters);
        Task<PagedList<TaskWithCategoryName>> GetTasksWithCategoryName();
		Task<Models.Task> GetTaskById(int id);
        Task<Models.Task> GetTaskByName(string name);
        Task<ICollection<Models.Task>> GetTasksByTaskCategory(int taskCategoryId);
        Task<bool> IsTaskExist(int id);
        Task<bool> AddTask(Models.Task task);
        Task<bool> UpdateTask(Models.Task task);
        Task<bool> DeleteTask(Models.Task task);
        Task<bool> Save();
        Task<CompetitionDto> GetCompetitionStatusFromNodeJS(int competitionId);
    }
}
