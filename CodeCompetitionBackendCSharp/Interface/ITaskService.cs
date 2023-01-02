namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITaskService
    {
        ICollection<Models.Task> GetTasks();
        Models.Task GetTaskById(int id);
        Models.Task GetTaskByName(string name);
        ICollection<Models.Task> GetTasksByTaskCategory(int taskCategoryId);
        bool IsTaskExist(int id);
        bool AddTask(Models.Task task);
        bool UpdateTask(Models.Task task);
        bool DeleteTask(Models.Task task);
        bool Save();
    }
}
