using CodeCompetitionBackendCSharp.Data;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.Paging;
using CodeCompetitionBackendCSharp.RequestFeatures;
using Microsoft.EntityFrameworkCore;

namespace CodeCompetitionBackendCSharp.Repository
{
    public class TaskCategoryService : ITaskCategoryService
    {
        private readonly DataContext _context;
        public TaskCategoryService(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> AddNewTaskCategory(TaskCategory taskCategory)
        {
            _context.Add(taskCategory);

            var result = await Save();
            return result;
        }

        public async Task<bool> DeleteTaskCategory(TaskCategory taskCategory)
        {
            _context.Attach(taskCategory);

            _context.Entry(taskCategory).Property(p => p.StatusId).IsModified = true;
            _context.Entry(taskCategory).Property(p => p.UpdateDate).IsModified = true;
            _context.Entry(taskCategory).Property(p => p.UpdateUserId).IsModified = true;
            
            var result = await Save();
            return result;
        }

        public async Task<PagedList<TaskCategory>> GetTaskCategories(TaskCategoryParameters taskCategoryParameters)
        {
            var result = await _context.TaskCategories
                                        .Where(tc => tc.StatusId == (int)Enum.Status.Active)
                                        .ToListAsync();
            return PagedList<TaskCategory>.ToPagedList(result, taskCategoryParameters.PageNumber, taskCategoryParameters.PageSize, taskCategoryParameters.SwitchOff);
        }

        public async Task<TaskCategory> GetTaskCategory(int id)
        {
            var result = await _context.TaskCategories
                                                    .Where(tc => tc.Id == id && tc.StatusId == (int)Enum.Status.Active)
                                                    .SingleOrDefaultAsync();
            return result;
        }

        public async Task<bool> IsTaskCategoryExist(int id)
        {
            var result = await _context.TaskCategories.AnyAsync(tc => tc.Id == id && tc.StatusId == (int)Enum.Status.Active);
            return result;
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            
            return saved > 0 ? true: false;
        }

        public async Task<bool> UpdateTaskCategory(TaskCategory taskCategory)
        {
            _context.Attach(taskCategory);

            _context.Entry(taskCategory).Property(p => p.CategoryName).IsModified = true;
            _context.Entry(taskCategory).Property(p => p.CategoryDescription).IsModified = true;
            _context.Entry(taskCategory).Property(p => p.UpdateDate).IsModified = true;
            _context.Entry(taskCategory).Property(p => p.UpdateUserId).IsModified = true;

            var result = await Save();
            return result;
        }
    }
}
