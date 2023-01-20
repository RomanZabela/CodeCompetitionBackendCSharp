using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.Repository;
using CodeCompetitionBackendCSharp.Services;
using Microsoft.AspNetCore.Mvc;

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

        public TaskToTeamController(
            ITaskToTeamService taskToTeamService, 
            ITeamService teamService,
            ITaskService taskService,
            IMapper mapper)
        {
            _taskToTeamService = taskToTeamService;
            _teamService = teamService;
            _taskService = taskService;
            _mapper = mapper;
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
                    return BadRequest(ModelState);
                }

                return Ok(teamsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet ("tasksCount/{teamId}")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<IActionResult> GetTeamTasksCount(int teamId)
        {
            try
            {
                var tasksNum = await _taskToTeamService.GetTaskCountByTeamId(teamId);
               
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return Ok(tasksNum);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateTeamTask([FromBody] TaskToTeamDto teamTaskCreate)
        {
            try
            {

                if (teamTaskCreate == null)
                    return BadRequest(ModelState);

                var teamTasks = await _taskToTeamService.GetTeamsTasks();

                var teamTasksFiltered = teamTasks.Where(c => (c.TeamId == teamTaskCreate.TeamId) && (c.TaskId == teamTaskCreate.TaskId)).FirstOrDefault();

                if (teamTasksFiltered != null)
                {
                    ModelState.AddModelError("", "This task for this team already exists");
                    return StatusCode(422, ModelState);
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var taskTeamMap = _mapper.Map<TaskToTeam>(teamTaskCreate);


                await _taskToTeamService.AddTeamsTasks(taskTeamMap);
                return Ok("Successfully created");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("deleteByTeamId/{teamId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteTeamTaskByTeamId([FromQuery] int newStatus, int teamId)
        {
            try
            {
                bool isExist = await _teamService.TeamExists(teamId);
                if (!isExist)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var tasksToTeams = await _taskToTeamService.GetTeamsTasks();
                var tasksToTeamsToDelete = tasksToTeams.Where(tt => tt.TeamId == teamId);

                foreach (var taskToTeam in tasksToTeamsToDelete)
                {
                    await _taskToTeamService.DeleteTeamTask((Enum.TaskStatus)newStatus, taskToTeam);
                }

                return NoContent();
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("deleteByTaskId/{taskId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteTeamTaskByTaskId([FromQuery] int newStatus, int taskId)
        {
            try
            {

                var isTaskExist = await _taskService.IsTaskExist(taskId);
                if (!isTaskExist)
                    return NotFound();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var tasksToTeams = await _taskToTeamService.GetTeamsTasks();
                var tasksToTeamsToDelete = tasksToTeams.Where(tt => tt.TaskId == taskId);

                foreach (var taskToTeam in tasksToTeamsToDelete)
                {
                    await _taskToTeamService.DeleteTeamTask((Enum.TaskStatus)newStatus, taskToTeam);
                }

                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteTeamTaskByTaskIdAndTeamId([FromQuery] int newStatus, [FromQuery] int taskId, [FromQuery] int teamId)
        {
            try
            {
                var isTaskExist = await _taskService.IsTaskExist(taskId);
                if (!isTaskExist)
                    return NotFound();

                var isTeamExist = await _teamService.TeamExists(teamId);
                if (!isTeamExist)
                    return NotFound();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var tasksToTeams = await _taskToTeamService.GetTeamsTasks();
                var tasksToTeamsToDelete = tasksToTeams.Where(tt => (tt.TaskId == taskId) && (tt.TeamId == teamId));

                foreach (var taskToTeam in tasksToTeamsToDelete)
                {
                    await _taskToTeamService.DeleteTeamTask((Enum.TaskStatus)newStatus, taskToTeam);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
