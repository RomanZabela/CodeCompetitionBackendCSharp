using CodeCompetitionBackendCSharp.Data;
using CodeCompetitionBackendCSharp.Interface;
using static CodeCompetitionBackendCSharp.Enum;

namespace CodeCompetitionBackendCSharp.Repository
{
    public class TaskService : ITaskService
    {
        private readonly DataContext _context;

        public TaskService(DataContext context)
        {
            _context= context;
        }

        public Models.Task GetTaskById(int id)
        {
            return _context.Tasks.Where(t => t.Id == id).FirstOrDefault();
        }

        public Models.Task GetTaskByName(string name)
        {
            return _context.Tasks.Where(t => t.TaskName.ToLower() == name.ToLower()).FirstOrDefault();
        }

        public ICollection<Models.Task> GetTasks()
        {
            return _context.Tasks.OrderBy(t => t.Id).ToList();
        }

        public bool IsTaskExist(int taskId)
        {
            return _context.Tasks.Any(t => t.Id == taskId && t.StatusId == (int)Status.Active);
        }
    }
}
