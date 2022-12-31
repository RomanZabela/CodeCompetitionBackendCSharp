using CodeCompetitionBackendCSharp.Data;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using static CodeCompetitionBackendCSharp.Enum;

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

        public ICollection<TaskCategory> GetTaskCategories()
        {
            return _context.TaskCategories.ToList();
        }

        public TaskCategory GetTaskCategory(int id)
        {
            return _context.TaskCategories.Where(tc => tc.Id == id).FirstOrDefault();
        }

        public bool IsTaskCategoryExist(int id)
        {
            return _context.TaskCategories.Any(tc => tc.Id == id && tc.StatusId == (int)Enum.Status.Active);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
