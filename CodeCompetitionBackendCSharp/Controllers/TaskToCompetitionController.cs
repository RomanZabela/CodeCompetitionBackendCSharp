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
            var competitionsIds = _taskToCompetitionService.GetTaskToCompetitionByTaskId(taskId);

            if (competitionsIds == null)
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
            var tasksIds = _taskToCompetitionService.GetTaskToCompetitionByCompetitionId(competitionId);

            if (tasksIds == null)
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
