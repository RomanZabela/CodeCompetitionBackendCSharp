namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITaskService
    {
        Task<ICollection<Models.Task>> GetTasks();
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
