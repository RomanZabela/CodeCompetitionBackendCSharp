using CodeCompetitionBackendCSharp.Models;

namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITaskCategoryService
    {
        ICollection<TaskCategory> GetTaskCategories();
        TaskCategory GetTaskCategory(int id);
        bool IsTaskCategoryExist(int id);
    }
}
