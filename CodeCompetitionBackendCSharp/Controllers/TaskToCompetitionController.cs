using CodeCompetitionBackendCSharp.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CodeCompetitionBackendCSharp.Controllers
{
    public class TaskToCompetitionController : Controller
    {
        private readonly ITaskToCompetitionService _taskToCompetitionService;

        public TaskToCompetitionController(ITaskToCompetitionService taskToCompetitionService)
        {
            _taskToCompetitionService = taskToCompetitionService;
        }

        [HttpGet("{taskId}/competitonIds")]
        [ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
        public IActionResult GetCompetitionsIdsByTaskId(int taskId)
        {
            var competitionsIds = _taskToCompetitionService.GetCompetitionIdsByTaskId(taskId);

            if (competitionsIds.Count() == 0)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(competitionsIds);
        }

        [HttpGet("{competitionId}/taskIds")]
        [ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
        public IActionResult GetTasksIdsByCompetitionId(int competitionId)
        {
            var tasksIds = _taskToCompetitionService.GetTasksIdsByCompetitionId(competitionId);

            if (tasksIds.Count() == 0)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(tasksIds);
        }
    }
}
