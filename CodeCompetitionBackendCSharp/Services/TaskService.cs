using CodeCompetitionBackendCSharp.Data;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;

namespace CodeCompetitionBackendCSharp.Repository
{
    public class TaskService : ITaskService
    {
        private readonly DataContext _context;

        public TaskService(DataContext context)
        {
            _context= context;
        }

        public bool AddTask(int competitionId, Models.Task task)
        {
            var taskCompetitionData = _context.Competitions.Where(c => c.Id == competitionId).FirstOrDefault();

            var taskCompetition = new TaskToCompetition()
            {
                Competition = taskCompetitionData,
                Task = task,
            };

            _context.Add(taskCompetitionData);
            _context.Add(task);
            return Save();
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
            return _context.Tasks.Any(t => t.Id == taskId && t.StatusId == (int)Enum.Status.Active);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
