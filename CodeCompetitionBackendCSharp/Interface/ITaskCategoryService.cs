using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.Paging;
using CodeCompetitionBackendCSharp.RequestFeatures;

namespace CodeCompetitionBackendCSharp.Interface
{
    public interface ITaskCategoryService
    {
        Task<PagedList<TaskCategory>> GetTaskCategories(TaskCategoryParameters taskCategoryParameters);
        Task<TaskCategory> GetTaskCategory(int id);
        Task<bool> IsTaskCategoryExist(int id);
        Task<bool> AddNewTaskCategory(TaskCategory taskCategory);
        Task<bool> UpdateTaskCategory(TaskCategory taskCategory);
        Task<bool> DeleteTaskCategory(TaskCategory taskCategory);
        Task<bool> Save();
    }
}
