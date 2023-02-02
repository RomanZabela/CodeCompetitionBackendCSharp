using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.Repository;
using CodeCompetitionBackendCSharp.RequestFeatures;
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
    public class TaskToTeamController : Controller
    {
        private readonly ITaskToTeamService _taskToTeamService;
        private readonly ITeamService _teamService;
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public TaskToTeamController(
            ITaskToTeamService taskToTeamService,
            ITeamService teamService,
            ITaskService taskService,
            IMapper mapper,
            ILoggerManager logger)
        {
            _taskToTeamService = taskToTeamService;
            _teamService = teamService;
            _taskService = taskService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
		[Authorize(Roles = "system administrator, competition administrator, participant")]
		[ProducesResponseType(200, Type = typeof(List<Team>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTeamsTasks()
        {
            _logger.LogDebug($"GetTeamsTasks started.");

            try
            {
                var teams = await _taskToTeamService.GetTeamsTasks();
                ICollection<TaskToTeamDto> teamsDto = _mapper.Map<ICollection<TaskToTeamDto>>(teams);

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"400. Bad Request. {ModelState}");
                    return BadRequest(ModelState);
                }

                _logger.LogInfo($"200. TaskToTeam: {JsonConvert.SerializeObject(teamsDto)}");
                return Ok(teamsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"500. {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

		[HttpGet("byTaskIdAndTeamIdAndCompetitionId/{taskId}/{teamId}/{competitionId}")]
		[Authorize(Roles = "system administrator, competition administrator, participant")]
		[ProducesResponseType(200, Type = typeof(List<Team>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetTeamTasksByTaskIdAndTeamIdAndCompetitionId(int taskId, int teamId, int competitionId)
		{
			_logger.LogDebug($"GetTeamTasksByTeamIdAndCompetitionId started with parameters task id: {taskId}, team id: {teamId}, competition id: {competitionId}");

			try
			{
				var teams = await _taskToTeamService.GetTeamTasksByTaskIdAndTeamIdAndCompetitionId(taskId, teamId, competitionId);
				ICollection<TaskToTeamDto> teamsDto = _mapper.Map<ICollection<TaskToTeamDto>>(teams);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				_logger.LogInfo($"200. TaskToTeam: {JsonConvert.SerializeObject(teamsDto)}");
				return Ok(teamsDto);
			}
			catch (Exception ex)
			{
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
			}
		}

		[HttpGet("tasksCount/{teamId}")]
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTeamTasksCount(int teamId)
        {
            _logger.LogDebug($"GetTeamTasksCount teamID: {teamId} started.");

            try
            {
                var tasksNum = await _taskToTeamService.GetTaskCountByTeamId(teamId);

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"400. Bad Request. {ModelState}");
                    return BadRequest(ModelState);
                }

                _logger.LogInfo($"200. TaskToTeam: {JsonConvert.SerializeObject(tasksNum)}");
                return Ok(tasksNum);
            }
            catch (Exception ex)
            {
                _logger.LogError($"500. {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateTeamTask([FromBody] TaskToTeamDto teamTaskCreate)
        {
            _logger.LogDebug($"CreateTeamTask TaskToTeamDto: {JsonConvert.SerializeObject(teamTaskCreate)} started.");

            if (teamTaskCreate == null)
                return BadRequest(ModelState);
            try
            {
                var teamTasks = await _taskToTeamService.GetTeamsTasks();

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"400. Bad Request. {ModelState}");
                    return BadRequest(ModelState);
                }

                var teamTasksFiltered = teamTasks.Where(c => (c.TeamId == teamTaskCreate.TeamId) && (c.TaskId == teamTaskCreate.TaskId)).FirstOrDefault();

                if (teamTasksFiltered != null)
                {
                    _logger.LogError($"409. This task {teamTaskCreate.TaskId} for this team {teamTaskCreate.TeamId} already exists.");
                    return Conflict("This task for this team already exists");
                }

                var taskTeamMap = _mapper.Map<TaskToTeam>(teamTaskCreate);

                await _taskToTeamService.AddTeamsTasks(taskTeamMap);

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"400. Bad Request. {ModelState}");
                    return BadRequest(ModelState);
                }

                _logger.LogInfo($"200. TaskToTeam {taskTeamMap.Id} created.");
                return Ok("Successfully created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"500. {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("deleteByTeamId/{teamId}")]
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteTeamTaskByTeamId([FromQuery] int newStatus, int teamId)
        {
            _logger.LogDebug($"DeleteTeamTaskByTeamId newStatus: {newStatus}, teamID: {teamId} started.");

            try
            {
                bool isExist = await _teamService.TeamExists(teamId);

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"400. Bad Request. {ModelState}");
                    return BadRequest(ModelState);
                }

                if (!isExist)
                {
                    _logger.LogError($"404. TaskToTeam with TaskId {teamId} couldn't found.");
                    return NotFound();
                }

                var tasksToTeams = await _taskToTeamService.GetTeamsTasks();

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"400. Bad Request. {ModelState}");
                    return BadRequest(ModelState);
                }

                var tasksToTeamsToDelete = tasksToTeams.Where(tt => tt.TeamId == teamId);

                foreach (var taskToTeam in tasksToTeamsToDelete)
                {

                    await _taskToTeamService.DeleteTeamTask((Enum.TaskStatus)newStatus, taskToTeam);

                    if (!ModelState.IsValid)
                    {
                        _logger.LogError($"400. Bad Request. {ModelState}");
                        return BadRequest(ModelState);
                    }

                }

                _logger.LogInfo($"204. TaskToTeam with TeamId {teamId} deleted.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"500. {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("deleteByTaskId/{taskId}")]
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteTeamTaskByTaskId([FromQuery] int newStatus, int taskId)
        {
            _logger.LogDebug($"DeleteTeamTaskByTaskId newStatus: {newStatus}, taskID: {taskId} started.");

            try
            {
                var isTaskExist = await _taskService.IsTaskExist(taskId);

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"400. Bad Request. {ModelState.ToString()}");
                    return BadRequest(ModelState);
                };

                if (!isTaskExist)
                {
                    _logger.LogError($"404. TaskToTeam with TaskId {taskId} couldn't found.");
                    return NotFound();
                }

                var tasksToTeams = await _taskToTeamService.GetTeamsTasks();

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"400. Bad Request. {ModelState.ToString()}");
                    return BadRequest(ModelState);
                };

                var tasksToTeamsToDelete = tasksToTeams.Where(tt => tt.TaskId == taskId);

                foreach (var taskToTeam in tasksToTeamsToDelete)
                {
                    await _taskToTeamService.DeleteTeamTask((Enum.TaskStatus)newStatus, taskToTeam);

                    if (!ModelState.IsValid)
                    {
                        _logger.LogError($"400. Bad Request. {ModelState.ToString()}");
                        return BadRequest(ModelState);
                    };

                };

                _logger.LogInfo($"204. TaskToTeam with TaskId {taskId} deleted.");
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
        [ProducesResponseType(401)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]

        public async Task<IActionResult> DeleteTeamTaskByTaskIdAndTeamId([FromQuery] int newStatus, [FromQuery] int taskId, [FromQuery] int teamId)

        {
            _logger.LogDebug($"DeleteTeamTaskByTaskIdAndTeamId newStatus: {newStatus}, taskID: {taskId}, teamID: {teamId} started.");

            try
            {
                var isTaskExist = await _taskService.IsTaskExist(taskId);

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"400. Bad Request. {ModelState}");
                    return BadRequest(ModelState);
                };

                if (!isTaskExist)
                {
                    _logger.LogError($"404. Task {taskId} couldn't found.");
                    return NotFound();
                }

                var isTeamExist = await _teamService.TeamExists(teamId);

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"400. Bad Request. {ModelState}");
                    return BadRequest(ModelState);
                };

                if (!isTeamExist)
                {
                    _logger.LogError($"404. Team {teamId} couldn't found.");
                    return NotFound();
                };

                var tasksToTeams = await _taskToTeamService.GetTeamsTasks();

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"400. Bad Request. {ModelState}");
                    return BadRequest(ModelState);
                };

                var tasksToTeamsToDelete = tasksToTeams.Where(tt => (tt.TaskId == taskId) && (tt.TeamId == teamId));

                foreach (var taskToTeam in tasksToTeamsToDelete)
                {
                    await _taskToTeamService.DeleteTeamTask((Enum.TaskStatus)newStatus, taskToTeam);
                }

                _logger.LogInfo($"204. TaskToTeam with TaskId {taskId} and TeamId {teamId} deleted.");
                return NoContent();
            }

            catch (Exception ex)
            {
                _logger.LogError($"500. {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

		[HttpPut("submit")]
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> SubmitTask([FromQuery] int taskId, [FromQuery] int teamId, [FromBody] SubmitTaskDataDto submitTaskDataDto)
		{
            _logger.LogDebug($"SubmitTask TaskID: {taskId}, teamID: {teamId}, SubmitTaskDataDto{JsonConvert.SerializeObject(submitTaskDataDto)} started.");

            if (submitTaskDataDto == null)
            {
				_logger.LogError($"400. Bad Request. {ModelState}");
				return BadRequest(ModelState);
			}

            try
            {
				var isTaskExist = await _taskService.IsTaskExist(taskId);
				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				};

				if (!isTaskExist)
                {
					_logger.LogError($"404. Task {taskId} couldn't found.");
					return NotFound();
				}
					
			}
			catch (Exception ex)
			{
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
			}

            try
            {
				var isTeamExist = await _teamService.TeamExists(teamId);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				};

				if (!isTeamExist)
                {
					_logger.LogError($"404. Team {teamId} couldn't found.");
					return NotFound();
				}
					
			}
			catch (Exception ex)
			{
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
			}

            try
            {
                var tasksToTeams = await _taskToTeamService.GetTeamsTasks();

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"400. Bad Request. {ModelState}");
                    return BadRequest(ModelState);
                };

                var tasksToTeamsToDelete = tasksToTeams.Where(tt => (tt.TaskId == taskId) && (tt.TeamId == teamId));

                foreach (var taskToTeam in tasksToTeamsToDelete)
                {
                    taskToTeam.GithubUrl = submitTaskDataDto.GithubUrl;
                    taskToTeam.SubmitterComment = submitTaskDataDto.SubmitterComment;

                    await _taskToTeamService.SubmitTask(taskToTeam);

                    if (!ModelState.IsValid)
                    {
                        _logger.LogError($"400. Bad Request. {ModelState}");
                        return BadRequest(ModelState);
                    };
                };

				_logger.LogInfo($"204. TaskToTeam with TaskId {taskId} and TeamId {teamId} submitted.");
				return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"500. {ex.Message}");
                return StatusCode(500, ex.Message);
            }
		}

        [HttpPut("evaluate")]
        [Authorize(Roles = "system administrator, competition administrator, participant")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> EvaluateTask([FromBody] EvaluateTaskDataDto evaluateTaskDataDto)
        {
            _logger.LogDebug($"Evaluate Task method with parameter evaluateTaskDataDto = {evaluateTaskDataDto} started.");

            if (evaluateTaskDataDto == null)
            {
                _logger.LogError($"400. Bad Request. {ModelState}");
                return BadRequest(ModelState);
            }

            try
            {
                var isTaskExist = await _taskService.IsTaskExist(evaluateTaskDataDto.TaskId);
                if (!ModelState.IsValid)
                {
                    _logger.LogError($"400. Bad Request. {ModelState}");
                    return BadRequest(ModelState);
                };

                if (!isTaskExist)
                {
                    _logger.LogError($"404. Task {evaluateTaskDataDto.TaskId} hasn't been found.");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"500. {ex.Message}");
                return StatusCode(500, ex.Message);
            }

            try
            {
                var isTeamExist = await _teamService.TeamExists(evaluateTaskDataDto.TeamId);

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"400. Bad Request. {ModelState}");
                    return BadRequest(ModelState);
                };

                if (!isTeamExist)
                {
                    _logger.LogError($"404. Team {evaluateTaskDataDto.TeamId} hasn't been found.");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"500. {ex.Message}");
                return StatusCode(500, ex.Message);
            }

			try
            {
              
				var tasksToTeams = await _taskToTeamService.GetTeamsTasks();

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"400. Bad Request. {ModelState}");
                    return BadRequest(ModelState);
                };

                var tasksToTeamsToEvaluate = await _taskToTeamService.GetTeamTasksByTaskIdAndTeamIdAndCompetitionId(evaluateTaskDataDto.TaskId, evaluateTaskDataDto.TeamId, evaluateTaskDataDto.CompetitionId);

                foreach (var taskToTeam in tasksToTeamsToEvaluate)
                {
                    taskToTeam.ReachedScore = evaluateTaskDataDto.Score;
                    taskToTeam.TaskStatusId = evaluateTaskDataDto.TaskStatusId;
                    taskToTeam.CompetitionAdminComment = evaluateTaskDataDto.Comment;

                    await _taskToTeamService.EvaluateTask(taskToTeam);

                    if (!ModelState.IsValid)
                    {
                        _logger.LogError($"400. Bad Request. {ModelState}");
                        return BadRequest(ModelState);
                    }
                };

                _logger.LogInfo($"204. TaskToTeam with TaskId {evaluateTaskDataDto.TaskId} and TeamId {evaluateTaskDataDto.TeamId} is evaluated.");
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

