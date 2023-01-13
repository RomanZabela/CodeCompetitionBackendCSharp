using CodeCompetitionBackendCSharp.Data;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.Paging;
using CodeCompetitionBackendCSharp.RequestFeatures;
using CodeCompetitionBackendCSharp.Services.ServiceExtensions;
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

        public async Task<PagedList<Models.Task>> GetTasks(TaskParameters taskParameters)
        {
            var result = await _context.Tasks.Where(t => t.StatusId == (int)Enum.Status.Active)
                                .OrderBy(t => t.Id)
                                .ToListAsync();
            return PagedList<Models.Task>.ToPagedList(result, taskParameters.PageNumber, taskParameters.PageSize);
        }

        public async Task<ICollection<Models.Task>> GetTasksByTaskCategory(int taskCategoryId)
        {
            var result = await _context.Tasks.Where(t => t.TaskCategoryId == taskCategoryId
                                                    && t.StatusId == (int)Enum.Status.Active)
                                            .ToListAsync();
            return result;
        }

		public async Task<PagedList<TaskWithCategoryNameDto>> GetTasksExtended(TaskParameters taskParameters)
		{
			var results = await (from t in _context.Tasks.Where(t => t.StatusId == (int)Enum.Status.Active)
                                 join tc in _context.TaskCategories on t.TaskCategoryId equals tc.Id
                                 select new TaskWithCategoryNameDto
								 {
						             Id = t.Id, 
                                     TaskName = t.TaskName, 
                                     TaskDescription = t.TaskDescription, 
                                     TaskContent = t.TaskContent, 
                                     Timeframe = t.Timeframe,
                                     Points = t.Points, 
                                     TaskCategoryName = tc.CategoryName 
                                 })
                                 .Search(taskParameters.SearchString)
                                 .Sort(taskParameters.OrderBy)
                                 .ToListAsync();

			return PagedList<TaskWithCategoryNameDto>.ToPagedList(results, taskParameters.PageNumber, taskParameters.PageSize);          
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
            _context.Entry(task).Property(p => p.Points).IsModified = true;
            _context.Entry(task).Property(p => p.Timeframe).IsModified = true;

            return Save();
        }
    }
}
