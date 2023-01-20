using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CodeCompetitionBackendCSharp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TaskToCompetitionController : Controller
    {
        private readonly ITaskToCompetitionService _taskToCompetitionService;
        private readonly IMapper _mapper;
		private readonly ILoggerManager _logger;

		public TaskToCompetitionController(ITaskToCompetitionService taskToCompetitionService,
                                            IMapper mapper,
                                            ILoggerManager logger)
        {
            _taskToCompetitionService = taskToCompetitionService;
            _mapper = mapper;
			_logger = logger;
		}

        [HttpGet("{taskId}/competitonIds")]
        [ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> GetCompetitionsIdsByTaskId(int taskId)
        {
            try
            {
                var competitionsIds = await _taskToCompetitionService.GetTaskToCompetitionByTaskId(taskId);

                if (competitionsIds == null)
                {
                    _logger.LogError($"404. TaskToCompetition with taskid: {taskId} couldn't found.");
                    return NotFound();
                }

                _logger.LogInfo($"200. TaskToCompetition: {competitionsIds.Count}.");
                return Ok(competitionsIds);
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}.");
				return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("{competitionId}/taskIds")]
        [ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> GetTasksIdsByCompetitionId(int competitionId)
        {
            try
            {
                var tasksIds = await _taskToCompetitionService.GetTaskToCompetitionByCompetitionId(competitionId);
                var taskToCompetition = _mapper.Map<List<TaskToCompetitionDto>>(tasksIds);

                if (tasksIds == null)
                {
					_logger.LogError($"404. TaskToCompetition with competitionId: {competitionId} couldn't found.");
					return NotFound();
                }

				_logger.LogInfo($"200. TaskToCompetition: {taskToCompetition.Count}.");
				return Ok(taskToCompetition);
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }

        }

        public class UserId
        {
            public int userId { get; set; }
        }

        [HttpDelete("{competitionId}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTaskToCompetitionById(int competitionId, [FromBody] int userId)
        {
            try
            {
                var taskToCompetitions = await _taskToCompetitionService.GetTaskToCompetitionByCompetitionId(competitionId);
                
                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
                }

                if (taskToCompetitions.Count == 0)
                {
                    _logger.LogInfo($"204. TaskToCompetition with competitionId {competitionId} couldn't found.");
                    return NoContent();
                }

                foreach(TaskToCompetition taskToCompetition in taskToCompetitions)
                {
                    taskToCompetition.UpdateDate = DateTime.Now;
                    taskToCompetition.StatusId = (int)Enum.Status.NotActive;
                    taskToCompetition.UpdateUserId = userId;
                }

                await _taskToCompetitionService.DeleteTaskToCompetition(taskToCompetitions);

                _logger.LogInfo($"204. TaskToCompetition with competitionId {competitionId} deleted.");
                return NoContent();
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }
        }
    }
}
