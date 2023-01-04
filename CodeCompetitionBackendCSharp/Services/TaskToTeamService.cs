using CodeCompetitionBackendCSharp.Data;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeCompetitionBackendCSharp.Services
{
    public class TaskToTeamService : ITaskToTeamService
    {
        private readonly DataContext _context;

        public TaskToTeamService(DataContext context)
        {
            _context = context;
        }

        async public Task<bool> AddTeamsTasks(TaskToTeam taskToTeam)
        {
            var curNumOfTasks = await GetTaskCountByTeamId(taskToTeam.TeamId);
            Console.WriteLine(curNumOfTasks);

            if (curNumOfTasks > 3) 
            {
                return false;
            }
            _context.Add(taskToTeam);
            return await Save();
        }
        async public Task<bool> DeleteTeamTask(Enum.TaskStatus newTaskStatus, TaskToTeam taskToTeam)
        {
            taskToTeam.UpdateDate = DateTime.Now;
            taskToTeam.UpdateUserId = Enum.TEMP_USER_ID;
            taskToTeam.TaskStatusId = (int)newTaskStatus;

            _context.Attach(taskToTeam);

            _context.Entry(taskToTeam).Property(tt => tt.UpdateDate).IsModified = true;
            _context.Entry(taskToTeam).Property(tt => tt.UpdateUserId).IsModified = true;
            _context.Entry(taskToTeam).Property(tt => tt.StatusId).IsModified = true;

            return await Save();
        }

        public async Task<int> GetTaskCountByTeamId(int teamId)
        {
           return await _context.TaskToTeams.Where(tt => (tt.TeamId == teamId) && (tt.TaskStatusId == (int)Enum.TaskStatus.InProgress)).CountAsync(); 
        }

        async public Task<ICollection<TaskToTeam>> GetTeamsTasks()
        {
            var tasksToTeams = await _context.TaskToTeams.Where(tt => tt.StatusId == (int)Enum.Status.Active).ToListAsync();
            return tasksToTeams;
        }

        async public Task<bool> Save()
        {
            int saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
    }
}
