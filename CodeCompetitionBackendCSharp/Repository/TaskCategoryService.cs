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
    }
}
