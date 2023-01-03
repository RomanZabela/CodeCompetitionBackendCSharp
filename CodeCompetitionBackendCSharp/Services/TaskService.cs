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

        public bool AddTask(Models.Task task)
        {
            //var taskCompetitionData = _context.Competitions.Where(c => c.Id == competitionId).FirstOrDefault();

            //var taskCompetition = new TaskToCompetition()
            //{
            //    Competition = taskCompetitionData,
            //    Task = task,
            //};

            //_context.Add(taskCompetitionData);
            _context.Add(task);
            return Save();
        }

        public bool DeleteTask(Models.Task task)
        {
            _context.Attach(task);

            _context.Entry(task).Property(p => p.StatusId).IsModified = true;
            _context.Entry(task).Property(p => p.UpdateDate).IsModified = true;
            _context.Entry(task).Property(p => p.UpdateUserId).IsModified = true;

            return Save();
        }

        public Models.Task GetTaskById(int id)
        {
            return _context.Tasks.Where(t => t.Id == id && t.StatusId == (int)Enum.Status.Active).FirstOrDefault();
        }

        public Models.Task GetTaskByName(string name)
        {
            return _context.Tasks.Where(t => t.TaskName.ToLower() == name.ToLower() && t.StatusId == (int)Enum.Status.Active).FirstOrDefault();
        }

        public ICollection<Models.Task> GetTasks()
        {
            return _context.Tasks.Where(t => t.StatusId == (int)Enum.Status.Active).OrderBy(t => t.Id).ToList();
        }

        public ICollection<Models.Task> GetTasksByTaskCategory(int taskCategoryId)
        {
            return _context.Tasks.Where(t => t.TaskCategoryId == taskCategoryId && t.StatusId == (int)Enum.Status.Active).ToList();
        }

        public bool IsTaskExist(int taskId)
        {
            return _context.Tasks.Any(t => t.Id == taskId && t.StatusId == (int)Enum.Status.Active);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            if (saved > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateTask(Models.Task task)
        {
            _context.Attach(task);

            _context.Entry(task).Property(p => p.TaskName).IsModified = true;
            _context.Entry(task).Property(p => p.TaskDescription).IsModified = true;
            _context.Entry(task).Property(p => p.TaskContent).IsModified = true;
            _context.Entry(task).Property(p => p.TaskCategoryId).IsModified = true;
            _context.Entry(task).Property(p => p.UpdateDate).IsModified = true;
            _context.Entry(task).Property(p => p.UpdateUserId).IsModified = true;

            return Save();
        }
    }
}
