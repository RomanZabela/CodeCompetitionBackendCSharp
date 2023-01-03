using CodeCompetitionBackendCSharp.Data;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;

namespace CodeCompetitionBackendCSharp.Services
{
    public class TaskToCompetitionService : ITaskToCompetitionService
    {
        private readonly DataContext _context;

        public TaskToCompetitionService(DataContext context)
        {
            _context = context;
        }

        public bool AddConnectionTaskIdAndCompetionId(TaskToCompetition taskToCompetition)
        {
            _context.Add(taskToCompetition);
            return Save();
        }

        public bool DeleteTaskToCompetitionConnection(TaskToCompetition taskToCompetition)
        {
            //_context.Remove(taskToCompetition);
            _context.Attach(taskToCompetition);

            _context.Entry(taskToCompetition).Property(p => p.UpdateUserId).IsModified = true;
            _context.Entry(taskToCompetition).Property(p => p.UpdateDate).IsModified = true;
            _context.Entry(taskToCompetition).Property(p => p.StatusId).IsModified = true;

            return Save();
        }

        public ICollection<TaskToCompetition> GetCompetitionIdsByTaskId(int taskId)
        {
            return _context.TaskToCompetitions.Where(ttc => ttc.TaskId == taskId && ttc.StatusId == (int)Enum.Status.Active).ToList();
        }

        public ICollection<TaskToCompetition> GetTasksIdsByCompetitionId(int competitionId)
        {
            return _context.TaskToCompetitions.Where(ttc => ttc.CompetitionId == competitionId && ttc.StatusId == (int)Enum.Status.Active).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();

            if (saved > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
