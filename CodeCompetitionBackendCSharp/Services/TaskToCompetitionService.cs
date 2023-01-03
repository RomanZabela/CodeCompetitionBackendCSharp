using CodeCompetitionBackendCSharp.Data;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeCompetitionBackendCSharp.Services
{
    public class TaskToCompetitionService : ITaskToCompetitionService
    {
        private readonly DataContext _context;

        public TaskToCompetitionService(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> AddTaskToCompetition(TaskToCompetition taskToCompetition)
        {
            await _context.AddAsync(taskToCompetition);

            var result = await Save();
            return result;
        }

        public async Task<bool> DeleteTaskToCompetition(TaskToCompetition taskToCompetition)
        {
            //_context.Remove(taskToCompetition);
            _context.Attach(taskToCompetition);

            _context.Entry(taskToCompetition).Property(p => p.UpdateUserId).IsModified = true;
            _context.Entry(taskToCompetition).Property(p => p.UpdateDate).IsModified = true;
            _context.Entry(taskToCompetition).Property(p => p.StatusId).IsModified = true;

            var result = await Save();

            return result;
        }

        public async Task<ICollection<TaskToCompetition>> GetTaskToCompetitionByTaskId(int taskId)
        {
            var result = await _context.TaskToCompetitions.Where(ttc => ttc.TaskId == taskId
                                                                && ttc.StatusId == (int)Enum.Status.Active)
                                                            .ToListAsync();
            return result;
        }

        public async Task<ICollection<TaskToCompetition>> GetTaskToCompetitionByCompetitionId(int competitionId)
        {
            var result = await _context.TaskToCompetitions.Where(ttc => ttc.CompetitionId == competitionId
                                                                && ttc.StatusId == (int)Enum.Status.Active)
                                                            .ToListAsync();
            return result;
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();

            return saved > 0 ? true : false;
        }
    }
}
