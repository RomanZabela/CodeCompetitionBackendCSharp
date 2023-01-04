using CodeCompetitionBackendCSharp.Data;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeCompetitionBackendCSharp.Repository
{
    public class TaskService : ITaskService
    {
        private readonly DataContext _context;

        public TaskService(DataContext context)
        {
            _context= context;
        }

        public Task<bool> AddTask(Models.Task task)
        {
            _context.Add(task);
            return Save();
        }

        public Task<bool> DeleteTask(Models.Task task)
        {
            _context.Attach(task);

            _context.Entry(task).Property(p => p.StatusId).IsModified = true;
            _context.Entry(task).Property(p => p.UpdateDate).IsModified = true;
            _context.Entry(task).Property(p => p.UpdateUserId).IsModified = true;

            return Save();
        }

        public Task<int> GetCompetitionStatusFromNodeJS(int competitionId)
        {
            throw new NotImplementedException();
        }

        public async Task<Models.Task> GetTaskById(int id)
        {
            var result = await _context.Tasks.Where(t => t.Id == id &&
                                                    t.StatusId == (int)Enum.Status.Active)
                                            .SingleOrDefaultAsync();
            return result;
        }

        public async Task<Models.Task> GetTaskByName(string name)
        {
            var result = await _context.Tasks.Where(t => t.TaskName.ToLower() == name.ToLower() &&
                                                    t.StatusId == (int)Enum.Status.Active)
                                            .SingleOrDefaultAsync();
            return result;
        }

        public async Task<ICollection<Models.Task>> GetTasks()
        {
            var result = await _context.Tasks.Where(t => t.StatusId == (int)Enum.Status.Active)
                                .OrderBy(t => t.Id)
                                .ToListAsync();
            return result;
        }

        public async Task<ICollection<Models.Task>> GetTasksByTaskCategory(int taskCategoryId)
        {
            var result = await _context.Tasks.Where(t => t.TaskCategoryId == taskCategoryId
                                                    && t.StatusId == (int)Enum.Status.Active)
                                            .ToListAsync();
            return result;
        }

        public async Task<bool> IsTaskExist(int taskId)
        {
            var result = await _context.Tasks.AnyAsync(t => t.Id == taskId
                                                    && t.StatusId == (int)Enum.Status.Active);
            return result;
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public Task<bool> UpdateTask(Models.Task task)
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
