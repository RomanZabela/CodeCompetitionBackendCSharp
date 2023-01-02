using CodeCompetitionBackendCSharp.Models;

namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITaskCategoryService
    {
        ICollection<TaskCategory> GetTaskCategories();
        TaskCategory GetTaskCategory(int id);
        bool IsTaskCategoryExist(int id);
        bool AddNewTaskCategory(TaskCategory taskCategory);
        bool UpdateTaskCategory(TaskCategory taskCategory);
        bool DeleteTaskCategory(TaskCategory taskCategory);
        bool Save();
    }
}
