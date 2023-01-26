using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CodeCompetitionBackendCSharp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    // [Authorize]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;
        private readonly ITaskCategoryService _taskCategoryService;
        private readonly ITaskToCompetitionService _taskToCompetitionService;

        public TaskController(ITaskService taskService,
            IMapper mapper,
            ITaskCategoryService taskCategoryService,
            ITaskToCompetitionService taskToCompetitionService)
        {
            _taskService = taskService;
            _mapper = mapper;
            _taskCategoryService = taskCategoryService;
            _taskToCompetitionService = taskToCompetitionService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Models.Task>))]
        public async Task<IActionResult> GetTasks([FromQuery] TaskParameters taskParameters )
        {
            try
            {
                var taskAsync = await _taskService.GetTasks(taskParameters);
                var tasks = _mapper.Map<List<TaskDto>>(taskAsync);
				Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(taskAsync.MetaData));

				return Ok(tasks);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }


		[HttpGet("extended")]
        [Authorize(Roles = "system administrator, competition administrator, participant")]

        [ProducesResponseType(200, Type = typeof(IEnumerable<Models.Task>))]
		public async Task<IActionResult> GetTasksExtended([FromQuery] TaskParameters taskParameters)
		{
			try
			{
                taskParameters.switchOff = taskParameters.switchOffString == "1" ? true : false;

                var taskAsync = await _taskService.GetTasksExtended(taskParameters);
				
				Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(taskAsync.MetaData));

				return Ok(taskAsync);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

        [HttpGet("byTeamId/{teamId}")]
        [Authorize(Roles = "system administrator, competition administrator, participant")]

        [ProducesResponseType(200, Type = typeof(IEnumerable<Models.Task>))]
        public async Task<IActionResult> GetTasksByTeamId([FromQuery] TaskParameters taskParameters, int teamId)
        {
            try
            {
                taskParameters.switchOff = taskParameters.switchOffString == "1" ? true : false;

                var taskAsync = await _taskService.GetTasksByTeamIdInProgress(taskParameters, teamId);

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(taskAsync.MetaData));

                return Ok(taskAsync);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("submitted/byTeamId/{teamId}")]
        [Authorize(Roles = "system administrator, competition administrator, participant")]

        [ProducesResponseType(200, Type = typeof(IEnumerable<Models.Task>))]
        public async Task<IActionResult> GetSubmittedTasksByTeamId([FromQuery] TaskParameters taskParameters, int teamId)
        {
            try
            {
                taskParameters.switchOff = taskParameters.switchOffString == "1" ? true : false;

                var taskAsync = await _taskService.GetTasksByTeamIdSubmitted(taskParameters, teamId);

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(taskAsync.MetaData));

                return Ok(taskAsync);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("byCompetitionId/{competitionId}")]
		[Authorize(Roles = "system administrator, competition administrator")]

		[ProducesResponseType(200, Type = typeof(IEnumerable<Models.Task>))]
		public async Task<IActionResult> GetTasksByCompetitionId([FromQuery] TaskParameters taskParameters, int competitionId)
		{
			try
			{
                taskParameters.switchOff = taskParameters.switchOffString == "1" ? true : false;

                var taskAsync = await _taskService.GetTasksByCompetitionId(taskParameters, competitionId);

				Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(taskAsync.MetaData));

				return Ok(taskAsync);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}

		}

		[HttpGet("{taskId}")]
        [ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTaskById(int taskId)
        {
            try
            {
                var taskExist = await _taskService.IsTaskExist(taskId);

                if (!taskExist)
                {
                    return NotFound();
                }

                var taskAsync = await _taskService.GetTaskById(taskId);
                var task = _mapper.Map<TaskDto>(taskAsync);

                return Ok(task);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{taskCategoryId}/Tasks")]
        [ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTaskByTaskCategory(int taskCategoryId)
        {
            try
            {
                var taskByCategory = await _taskService.GetTasksByTaskCategory(taskCategoryId);

                if (taskByCategory == null)
                {
                    return NotFound();
                }

                var taskMap = _mapper.Map<List<TaskDto>>(taskByCategory);

                return Ok(taskMap);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddNewTask([FromBody] TaskDto task)
        {
            if (task == null)
            {
                return BadRequest(ModelState);
            }

            try
            {
                TaskParameters taskParameters = new TaskParameters();
                taskParameters.switchOff = true;

				var taskExist = (await _taskService.GetTasks(taskParameters))
                                                    .Where(te => te.TaskName.Trim().ToLower() == task.TaskName.Trim().ToLower()).ToList();

                if (taskExist.Count > 0)
                {
                    ModelState.AddModelError("", "Task with the same name already exist in DB");
                    return StatusCode(422, ModelState);
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var taskMap = _mapper.Map<Models.Task>(task);

                taskMap.UpdateDate = DateTime.Now;
                taskMap.CreateDate = DateTime.Now;
                taskMap.UpdateUserId = Enum.TEMP_USER_ID;
                taskMap.CreateUserId = Enum.TEMP_USER_ID;
                taskMap.StatusId = (int)Enum.Status.Active;

                taskMap.TaskCategory = await _taskCategoryService.GetTaskCategory(task.TaskCategoryId);

                var newTask = await _taskService.AddTask(taskMap);

                return Ok("Task category created");
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            };
            
        }

        [HttpPut("{taskId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateTask(int taskId, [FromBody] TaskDto updateTask)
        {
            if (updateTask == null)
            {
                return BadRequest(ModelState);
            }

            if (taskId != updateTask.Id)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var taskExist = await _taskService.IsTaskExist(updateTask.Id);

                if (!taskExist)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var taskMap = _mapper.Map<Models.Task>(updateTask);

                taskMap.UpdateDate = DateTime.Now;
                taskMap.UpdateUserId = Enum.TEMP_USER_ID;
                taskMap.StatusId = (int)Enum.Status.Active;


                var updatedTask = await _taskService.UpdateTask(taskMap);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpDelete("{taskId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            try
            {
                var taskToDelete = await _taskService.GetTaskById(taskId);

                if (taskToDelete == null)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                taskToDelete.StatusId = (int)Enum.Status.NotActive;
                taskToDelete.UpdateDate = DateTime.Now;
                taskToDelete.UpdateUserId = (int)Enum.TEMP_USER_ID;

                var taskToCompetitions = await _taskToCompetitionService.GetTaskToCompetitionByTaskId(taskId);

                if (taskToCompetitions.Count > 0)
                {
                    foreach (var taskToCompetition in taskToCompetitions)
                    {
                        taskToCompetition.UpdateDate = DateTime.Now;
                        taskToCompetition.UpdateUserId = (int)Enum.TEMP_USER_ID;
                        taskToCompetition.StatusId = (int)Enum.Status.NotActive;
                    }

                    await _taskToCompetitionService.DeleteTaskToCompetition(taskToCompetitions);
                }

                await _taskService.DeleteTask(taskToDelete);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }
    }
}
