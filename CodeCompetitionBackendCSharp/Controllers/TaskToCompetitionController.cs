using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeCompetitionBackendCSharp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TaskToCompetitionController : Controller
    {
        private readonly ITaskToCompetitionService _taskToCompetitionService;
        private readonly IMapper _mapper;

        public TaskToCompetitionController(ITaskToCompetitionService taskToCompetitionService, IMapper mapper)
        {
            _taskToCompetitionService = taskToCompetitionService;
            _mapper = mapper;
        }

        [HttpGet("{taskId}/competitonIds")]
        [ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetCompetitionsIdsByTaskId(int taskId)
        {
            try
            {
                var competitionsIds = await _taskToCompetitionService.GetTaskToCompetitionByTaskId(taskId);

                if (competitionsIds == null)
                {
                    return NotFound();
                }

                return Ok(competitionsIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCompetitionTask([FromBody] TaskToCompetitionDto taskToCompetition)
        {

            if (taskToCompetition == null)
                return BadRequest(ModelState);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var competitionTasks = await _taskToCompetitionService.GetTasksToCompetitions();

            var competitionTasksFiltered = competitionTasks.Where(ct => (ct.TaskId == taskToCompetition.TaskId) && (ct.CompetitionId == taskToCompetition.CompetitionId)).FirstOrDefault();

            if (competitionTasksFiltered != null)
            {
                ModelState.AddModelError("", "This task for this competition already exists");
                return StatusCode(422, ModelState);
            }

            var taskToCompetitionMap = _mapper.Map<TaskToCompetition>(taskToCompetition);


            try
            {
                await _taskToCompetitionService.AddTaskToCompetition(taskToCompetitionMap);
                return Ok("Successfully created");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{competitionId}/taskIds")]
        [ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTasksIdsByCompetitionId(int competitionId)
        {
            try
            {
                var tasksIds = await _taskToCompetitionService.GetTaskToCompetitionByCompetitionId(competitionId);
                var taskToCompetition = _mapper.Map<List<TaskToCompetitionDto>>(tasksIds);

                if (tasksIds == null)
                {
                    return NotFound();
                }

                return Ok(taskToCompetition);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        public class UserId
        {
            public int userId { get; set; }
        }

        [HttpDelete("{competitionId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTaskToCompetitionById(int competitionId, [FromBody] int userId)
        {
            try
            {
                var taskToCompetitions = await _taskToCompetitionService.GetTaskToCompetitionByCompetitionId(competitionId);
                
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (taskToCompetitions.Count == 0)
                {
                    return NoContent();
                }

                foreach(TaskToCompetition taskToCompetition in taskToCompetitions)
                {
                    taskToCompetition.UpdateDate = DateTime.Now;
                    taskToCompetition.StatusId = (int)Enum.Status.NotActive;
                    taskToCompetition.UpdateUserId = userId;
                }

                await _taskToCompetitionService.DeleteTaskToCompetition(taskToCompetitions);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTaskToCompetition([FromQuery] int competitionId, [FromQuery] int taskId)
        {
            try
            {
                var taskToCompetitions = await _taskToCompetitionService.GetTaskToCompetitionByCompetitionId(competitionId);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (taskToCompetitions.Count == 0)
                {
                    return NoContent();
                }

                TaskToCompetition taskToCompetitionToDelete = taskToCompetitions.Where(ttc => ttc.TaskId == taskId).FirstOrDefault();

                await _taskToCompetitionService.DeleteTaskToCompetitionSingle(taskToCompetitionToDelete);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
