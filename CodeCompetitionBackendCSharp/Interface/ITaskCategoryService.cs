using CodeCompetitionBackendCSharp.Models;

namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITaskCategoryService
    {
        Task<ICollection<TaskCategory>> GetTaskCategories();
        Task<TaskCategory> GetTaskCategory(int id);
        Task<bool> IsTaskCategoryExist(int id);
        Task<bool> AddNewTaskCategory(TaskCategory taskCategory);
        Task<bool> UpdateTaskCategory(TaskCategory taskCategory);
        Task<bool> DeleteTaskCategory(TaskCategory taskCategory);
        Task<bool> Save();
    }
}
