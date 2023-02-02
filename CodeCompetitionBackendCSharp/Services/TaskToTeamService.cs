using CodeCompetitionBackendCSharp.Data;
using CodeCompetitionBackendCSharp.Dto;
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

            if (curNumOfTasks > 3) 
            {
                return false;
            }

            taskToTeam.UpdateDate = DateTime.Now;
            taskToTeam.CreateDate = DateTime.Now;
            taskToTeam.StartTime = DateTime.Now;
            taskToTeam.EndTime = DateTime.MaxValue;
            taskToTeam.CreateUserId = Enum.TEMP_USER_ID;
            taskToTeam.UpdateUserId = Enum.TEMP_USER_ID;
            taskToTeam.StatusId = (int)Enum.Status.Active;

            _context.Add(taskToTeam);
            return await Save();
        }
        async public Task<bool> DeleteTeamTask(Enum.TaskStatus newTaskStatus, TaskToTeam taskToTeam)
        {
            taskToTeam.UpdateDate = DateTime.Now;
            taskToTeam.EndTime = DateTime.Now;
            taskToTeam.UpdateUserId = Enum.TEMP_USER_ID;
            taskToTeam.TaskStatusId = (int)newTaskStatus;

            _context.Attach(taskToTeam);

            _context.Entry(taskToTeam).Property(tt => tt.UpdateDate).IsModified = true;
            _context.Entry(taskToTeam).Property(tt => tt.EndTime).IsModified = true;
            _context.Entry(taskToTeam).Property(tt => tt.UpdateUserId).IsModified = true;
			_context.Entry(taskToTeam).Property(tt => tt.TaskStatusId).IsModified = true;

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

		async public Task<ICollection<TaskToTeam>> GetTeamTasksByTaskIdAndTeamIdAndCompetitionId(int taskId, int teamId, int competitionId)
        {
			var result = await (from ttm in _context.TaskToTeams.Where(ttm => ttm.StatusId == (int)Enum.Status.Active && ttm.TaskId == taskId && ttm.TeamId == teamId)
								 join t in _context.Teams.Where(t => t.StatusId == (int)Enum.Status.Active && t.CompetitionId == competitionId) on ttm.TeamId equals t.Id
								 select new TaskToTeam
								 {
									 Id = t.Id,
									 TeamId = t.Id,
                                     TaskId = ttm.TaskId,
                                     TaskStatusId = ttm.TaskStatusId,
                                     StatusId = ttm.StatusId,
                                     ParticipantIdForTask = ttm.ParticipantIdForTask,
                                     CreateDate = ttm.CreateDate,
                                     UpdateDate = ttm.UpdateDate,
                                     StartTime = ttm.StartTime,
                                     EndTime = ttm.EndTime,
                                     CreateUserId = ttm.CreateUserId,
                                     UpdateUserId = ttm.UpdateUserId,
                                     ReachedScore = ttm.ReachedScore,
                                     GithubUrl = ttm.GithubUrl,
                                     SubmitterComment = ttm.SubmitterComment,
                                     CompetitionAdminComment = ttm.CompetitionAdminComment
								 }).ToListAsync();
            return result;
		}

		async public Task<bool> Save()
        {
            int saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

		public async Task<bool> SubmitTask(TaskToTeam taskToTeam)
		{
			taskToTeam.UpdateDate = DateTime.Now;
			taskToTeam.EndTime = DateTime.Now;
			taskToTeam.UpdateUserId = Enum.TEMP_USER_ID;
			taskToTeam.TaskStatusId = (int)Enum.TaskStatus.Submitted;

			_context.Attach(taskToTeam);

			_context.Entry(taskToTeam).Property(tt => tt.UpdateDate).IsModified = true;
			_context.Entry(taskToTeam).Property(tt => tt.EndTime).IsModified = true;
			_context.Entry(taskToTeam).Property(tt => tt.UpdateUserId).IsModified = true;
			_context.Entry(taskToTeam).Property(tt => tt.TaskStatusId).IsModified = true;
            _context.Entry(taskToTeam).Property(tt => tt.GithubUrl).IsModified = true;
            _context.Entry(taskToTeam).Property(tt => tt.SubmitterComment).IsModified = true;
            _context.Entry(taskToTeam).Property(tt => tt.CompetitionAdminComment).IsModified = true;

            return await Save();
		}

        public async Task<bool> EvaluateTask(TaskToTeam taskToTeam)
        {
            taskToTeam.UpdateDate = DateTime.Now;
            taskToTeam.UpdateUserId = Enum.TEMP_USER_ID;
           
            _context.Attach(taskToTeam);

            _context.Entry(taskToTeam).Property(tt => tt.UpdateDate).IsModified = true;
            _context.Entry(taskToTeam).Property(tt => tt.EndTime).IsModified = true;
            _context.Entry(taskToTeam).Property(tt => tt.UpdateUserId).IsModified = true;
            _context.Entry(taskToTeam).Property(tt => tt.TaskStatusId).IsModified = true;
            _context.Entry(taskToTeam).Property(tt => tt.CompetitionAdminComment).IsModified = true;
            _context.Entry(taskToTeam).Property(tt => tt.ReachedScore).IsModified = true;

            return await Save();
        }
    }
}
