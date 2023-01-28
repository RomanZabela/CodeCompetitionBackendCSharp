using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CodeCompetitionBackendCSharp.Controllers
{
    [Route("[controller]")]
    [ApiController]
	//[Authorize]
	public class TaskToCompetitionController : Controller
    {
        private readonly ITaskToCompetitionService _taskToCompetitionService;
        private readonly IMapper _mapper;
		private readonly ILoggerManager _logger;

		public TaskToCompetitionController(
                    ITaskToCompetitionService taskToCompetitionService,
                    IMapper mapper,
                    ILoggerManager logger)
        {
            _taskToCompetitionService = taskToCompetitionService;
            _mapper = mapper;
			_logger = logger;
		}

        [HttpGet("{taskId}/competitonIds")]
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetCompetitionsIdsByTaskId(int taskId)
        {
			_logger.LogDebug($"GetCompetitionsIdsByTaskId taskId: {taskId} started.");

            try
            {
                var competitionsIds = await _taskToCompetitionService.GetTaskToCompetitionByTaskId(taskId);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				if (competitionsIds == null)
                {
					_logger.LogError($"404. TaskToCompetition with taskid: {taskId} couldn't found.");
					return NotFound();
                }

				_logger.LogInfo($"200. TaskToCompetition: {JsonConvert.SerializeObject(competitionsIds)}.");
				return Ok(competitionsIds);
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}.");
				return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("{competitionId}/taskIds")]
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetTasksIdsByCompetitionId(int competitionId)
        {
			_logger.LogDebug($"GetTasksIdsByCompetitionId competitionId: {competitionId} started.");

            try
            {
                var tasksIds = await _taskToCompetitionService.GetTaskToCompetitionByCompetitionId(competitionId);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				var taskToCompetition = _mapper.Map<List<TaskToCompetitionDto>>(tasksIds);

                if (tasksIds == null)
                {
					_logger.LogError($"404. TaskToCompetition with competitionId: {competitionId} couldn't found.");
					return NotFound();
                }

				_logger.LogInfo($"200. TaskToCompetition: {JsonConvert.SerializeObject(taskToCompetition)}.");
				return Ok(taskToCompetition);
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }

        }

		[HttpPost]
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(200, Type = typeof(string))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(409)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> CreateCompetitionTask([FromBody] TaskToCompetitionDto taskToCompetition)
		{
			_logger.LogDebug($"CreateCompetitionTask TaskToCompetitionDto: {JsonConvert.SerializeObject(taskToCompetition)} started.");

			if (taskToCompetition == null)
            {
				_logger.LogError($"400. Bad Request. {ModelState}");
				return BadRequest(ModelState);
			}

            try
            {
				var competitionTasks = await _taskToCompetitionService.GetTasksToCompetitions();

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				var competitionTasksFiltered = competitionTasks.Where(ct => (ct.TaskId == taskToCompetition.TaskId) && (ct.CompetitionId == taskToCompetition.CompetitionId)).FirstOrDefault();

				if (competitionTasksFiltered != null)
                {
					_logger.LogError($"409. Task with the same name {taskToCompetition} already exist in DB");
					return Conflict("The same TaskToCompetition already exist in DB");
				}

				var taskToCompetitionMap = _mapper.Map<TaskToCompetition>(taskToCompetition);
			
				await _taskToCompetitionService.AddTaskToCompetition(taskToCompetitionMap);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				_logger.LogInfo($"200. TaskToCompetition {JsonConvert.SerializeObject(taskToCompetitionMap)}.");
				return Ok("Successfully created");
			}
			catch (Exception ex)
			{
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
			}
		}

		[HttpDelete("{competitionId}")]
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> DeleteTaskToCompetitionById(int competitionId, [FromBody] int userId)
        {
			_logger.LogDebug($"DeleteTaskToCompetitionById competitionID: {competitionId}, userID: {userId} started.");

            try
            {
                var taskToCompetitions = await _taskToCompetitionService.GetTaskToCompetitionByCompetitionId(competitionId);
                
                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState}");
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

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				_logger.LogInfo($"204. TaskToCompetition with competitionId {competitionId} deleted.");
				return NoContent();
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> DeleteTaskToCompetition([FromQuery] int competitionId, [FromQuery] int taskId)
        {
			_logger.LogDebug($"DeleteTaskToCompetition competitionId: {competitionId}, taskID: {taskId} started.");

            try
            {
                var taskToCompetitions = await _taskToCompetitionService.GetTaskToCompetitionByCompetitionId(competitionId);

                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
                }

                if (taskToCompetitions.Count == 0)
                {
					_logger.LogInfo($"204. TaskToCompetition with competitionId {competitionId} couldn't found.");
					return NoContent();
                }				

                TaskToCompetition? taskToCompetitionToDelete = taskToCompetitions.Where(ttc => ttc.TaskId == taskId).FirstOrDefault();

				if (taskToCompetitionToDelete == null)
				{
					_logger.LogInfo($"204. TaskToCompetition with competitionId {competitionId} couldn't found.");
					return NoContent();
				}

                await _taskToCompetitionService.DeleteTaskToCompetitionSingle(taskToCompetitionToDelete);

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
