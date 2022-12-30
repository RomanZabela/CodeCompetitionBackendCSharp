namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITaskService
    {
        ICollection<Models.Task> GetTasks();
        Models.Task GetTaskById(int id);
        Models.Task GetTaskByName(string name);
        bool IsTaskExist(int id);
    }
}
