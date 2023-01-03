using CodeCompetitionBackendCSharp.Data;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;

namespace CodeCompetitionBackendCSharp.Repository
{
    public class TaskCategoryService : ITaskCategoryService
    {
        private readonly DataContext _context;
        public TaskCategoryService(DataContext context)
        {
            _context = context;
        }

        public bool AddNewTaskCategory(TaskCategory taskCategory)
        {
            _context.Add(taskCategory);
            return Save();
        }

        public bool DeleteTaskCategory(TaskCategory taskCategory)
        {
            _context.Attach(taskCategory);

            _context.Entry(taskCategory).Property(p => p.StatusId).IsModified = true;
            _context.Entry(taskCategory).Property(p => p.UpdateDate).IsModified = true;
            _context.Entry(taskCategory).Property(p => p.UpdateUserId).IsModified = true;

            return Save();
        }

        public ICollection<TaskCategory> GetTaskCategories()
        {
            return _context.TaskCategories.ToList();
        }

        public TaskCategory GetTaskCategory(int id)
        {
            return _context.TaskCategories.Where(tc => tc.Id == id && tc.StatusId == (int)Enum.Status.Active).FirstOrDefault();
        }

        public bool IsTaskCategoryExist(int id)
        {
            return _context.TaskCategories.Any(tc => tc.Id == id && tc.StatusId == (int)Enum.Status.Active);
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

        public bool UpdateTaskCategory(TaskCategory taskCategory)
        {
            _context.Attach(taskCategory);

            _context.Entry(taskCategory).Property(p => p.CategoryName).IsModified = true;
            _context.Entry(taskCategory).Property(p => p.CategoryDescription).IsModified = true;
            _context.Entry(taskCategory).Property(p => p.UpdateDate).IsModified = true;
            _context.Entry(taskCategory).Property(p => p.UpdateUserId).IsModified = true;

            return Save();
        }
    }
}
