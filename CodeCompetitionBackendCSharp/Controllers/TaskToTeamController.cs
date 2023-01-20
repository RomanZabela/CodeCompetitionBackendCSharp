using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.Repository;
using CodeCompetitionBackendCSharp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CodeCompetitionBackendCSharp.Controllers
{
    [Route("[controller]")]
    [ApiController]
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
        [ProducesResponseType(200, Type = typeof(List<Models.Team>))]
        public async Task<IActionResult> GetTeamsTasks()
        {
            try
            {
                var teams = await _taskToTeamService.GetTeamsTasks();
                ICollection<TaskToTeamDto> teamsDto = _mapper.Map<ICollection<TaskToTeamDto>>(teams);

                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
                }

				_logger.LogInfo($"200. TaskToTeam: {teamsDto.Count}");
				return Ok(teamsDto);
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }
        }

        [HttpGet ("tasksCount/{teamId}")]
        [ProducesResponseType(200, Type = typeof(int))]
		[ProducesResponseType(401)]
		public async Task<IActionResult> GetTeamTasksCount(int teamId)
        {
            try
            {
                var tasksNum = await _taskToTeamService.GetTaskCountByTeamId(teamId);
               
                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
                }

                _logger.LogInfo($"200. TaskToTeam: {tasksNum}");
                return Ok(tasksNum);
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> CreateTeamTask([FromBody] TaskToTeamDto teamTaskCreate)
        {
            try
            {

                if (teamTaskCreate == null)
                {
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				}
                    

                var teamTasks = await _taskToTeamService.GetTeamsTasks();

                var teamTasksFiltered = teamTasks.Where(c => (c.TeamId == teamTaskCreate.TeamId) && (c.TaskId == teamTaskCreate.TaskId)).FirstOrDefault();

                if (teamTasksFiltered != null)
                {
                    _logger.LogError($"409. This task {teamTaskCreate.TaskId} for this team {teamTaskCreate.TeamId} already exists.");
                    return Conflict("This task for this team already exists");
                }

                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				}
                    

                var taskTeamMap = _mapper.Map<TaskToTeam>(teamTaskCreate);

                await _taskToTeamService.AddTeamsTasks(taskTeamMap);

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
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> DeleteTeamTaskByTeamId([FromQuery] int newStatus, int teamId)
        {
            try
            {
                bool isExist = await _teamService.TeamExists(teamId);
                if (!isExist)
                {
					_logger.LogError($"404. TaskToTeam with TaskId {teamId} couldn't found.");
					return NotFound();
                }

                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				};                    

                var tasksToTeams = await _taskToTeamService.GetTeamsTasks();
                var tasksToTeamsToDelete = tasksToTeams.Where(tt => tt.TeamId == teamId);

                foreach (var taskToTeam in tasksToTeamsToDelete)
                {
                    await _taskToTeamService.DeleteTeamTask((Enum.TaskStatus)newStatus, taskToTeam);
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
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> DeleteTeamTaskByTaskId([FromQuery] int newStatus, int taskId)
        {
            try
            {

                var isTaskExist = await _taskService.IsTaskExist(taskId);
                if (!isTaskExist)
                {
                    _logger.LogError($"404. TaskToTeam with TaskId {taskId} couldn't found.");
					return NotFound();
				}
                    

                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				};                 

                var tasksToTeams = await _taskToTeamService.GetTeamsTasks();
                var tasksToTeamsToDelete = tasksToTeams.Where(tt => tt.TaskId == taskId);

                foreach (var taskToTeam in tasksToTeamsToDelete)
                {
                    await _taskToTeamService.DeleteTeamTask((Enum.TaskStatus)newStatus, taskToTeam);
                }

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
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> DeleteTeamTaskByTaskIdAndTeamId([FromQuery] int newStatus, [FromQuery] int taskId, [FromQuery] int teamId)
        {
            try
            {
                var isTaskExist = await _taskService.IsTaskExist(taskId);
                if (!isTaskExist)
                {
					_logger.LogError($"404. Task {taskId} couldn't found.");
					return NotFound();
				}
                    

                var isTeamExist = await _teamService.TeamExists(teamId);

                if (!isTeamExist)
                {
                    _logger.LogError($"404. Team {teamId} couldn't found.");
					return NotFound();
				};
                    

                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				};                 

                var tasksToTeams = await _taskToTeamService.GetTeamsTasks();
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
    }
}
