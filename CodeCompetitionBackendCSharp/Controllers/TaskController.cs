using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

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
		private readonly ILoggerManager _logger;

		public TaskController(ITaskService taskService,
            IMapper mapper,
            ITaskCategoryService taskCategoryService,
            ITaskToCompetitionService taskToCompetitionService,
            ILoggerManager logger)
        {
            _taskService = taskService;
            _mapper = mapper;
            _taskCategoryService = taskCategoryService;
            _taskToCompetitionService = taskToCompetitionService;
			_logger = logger;
		}

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Models.Task>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetTasks([FromQuery] TaskParameters taskParameters )
        {
			_logger.LogDebug($"GetTasks TaskParameters:{JsonConvert.SerializeObject(taskParameters)} started");

            try
            {
                var taskAsync = await _taskService.GetTasks(taskParameters);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				var tasks = _mapper.Map<List<TaskDto>>(taskAsync);
				Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(taskAsync.MetaData));

				_logger.LogInfo($"200. All Tasks: {JsonConvert.SerializeObject(taskAsync)}");
				return Ok(tasks);
            }
            catch(Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }
            
        }


		[HttpGet("extended")]
        [Authorize(Roles = "system administrator, competition administrator, participant")]

        [ProducesResponseType(200, Type = typeof(IEnumerable<Models.Task>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetTasksExtended([FromQuery] TaskParameters taskParameters)
		{
			_logger.LogDebug($"GetTasksExtended TaskParameters: {JsonConvert.SerializeObject(taskParameters)} started");

			try
			{
                taskParameters.switchOff = taskParameters.switchOffString == "1" ? true : false;

                var taskAsync = await _taskService.GetTasksExtended(taskParameters);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(taskAsync.MetaData));

				_logger.LogInfo($"200. Task with {taskParameters.SearchString} {taskParameters.PageNumber} {taskParameters.OrderBy}: {JsonConvert.SerializeObject(taskAsync)}");
				return Ok(taskAsync);
			}
			catch (Exception ex)
			{
				_logger.LogError($"500. {ex.Message}");
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
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> GetTasksByCompetitionId([FromQuery] TaskParameters taskParameters, int competitionId)
		{
			_logger.LogDebug($"GetTasksByCompetitionId TaskParameters: {JsonConvert.SerializeObject(taskParameters)} competitionID: {competitionId} started.");

			if (taskParameters == null)
			{
				_logger.LogError($"400. Bad Request. {ModelState}");
				return BadRequest(ModelState);
			}

			try
			{
                taskParameters.switchOff = taskParameters.switchOffString == "1" ? true : false;

                var taskAsync = await _taskService.GetTasksByCompetitionId(taskParameters, competitionId);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(taskAsync.MetaData));

				_logger.LogInfo($"200. Task: {JsonConvert.SerializeObject(taskAsync)}");

				return Ok(taskAsync);
			}
			catch (Exception ex)
			{
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
			}

		}

		[HttpGet("{taskId}")]
        [ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetTaskById(int taskId)
        {
			_logger.LogDebug($"GetTaskById taskId: {taskId} started");

            try
            {
                var taskExist = await _taskService.IsTaskExist(taskId);

                if (!taskExist)
                {
					_logger.LogError($"404. Task {taskId} couldn't found.");
					return NotFound();
                }

                var taskAsync = await _taskService.GetTaskById(taskId);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				
				var task = _mapper.Map<TaskDto>(taskAsync);

				_logger.LogInfo($"200. Task: {JsonConvert.SerializeObject(task)}");
				return Ok(task);
            }
            catch(Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{taskCategoryId}/Tasks")]
        [ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetTaskByTaskCategory(int taskCategoryId)
        {
			_logger.LogDebug($"GetTaskByTaskCategory taskCategoryId: {taskCategoryId} started");

            try
            {
                var taskByCategory = await _taskService.GetTasksByTaskCategory(taskCategoryId);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				if (taskByCategory == null)
                {
					_logger.LogError($"404. Task by CategoryId {taskCategoryId} couldn't found.");
					return NotFound();
                }

                var taskMap = _mapper.Map<List<TaskDto>>(taskByCategory);

				_logger.LogInfo($"200. Task: {JsonConvert.SerializeObject(taskMap)}");
				return Ok(taskMap);
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
		[ProducesResponseType(409)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> AddNewTask([FromBody] TaskDto task)
        {
			_logger.LogDebug($"AddNewTask TaskDto: {JsonConvert.SerializeObject(task)}");

            if (task == null)
            {
				_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
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
					_logger.LogError($"409. Task with the same name {task.TaskName} already exist in DB");
					return Conflict("Task with the same name already exist in DB");
				}

                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
                }

                var taskMap = _mapper.Map<Models.Task>(task);

                taskMap.UpdateDate = DateTime.Now;
                taskMap.CreateDate = DateTime.Now;
                taskMap.UpdateUserId = Enum.TEMP_USER_ID;
                taskMap.CreateUserId = Enum.TEMP_USER_ID;
                taskMap.StatusId = (int)Enum.Status.Active;

                taskMap.TaskCategory = await _taskCategoryService.GetTaskCategory(task.TaskCategoryId);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				}

				var newTask = await _taskService.AddTask(taskMap);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				}

				_logger.LogInfo($"200. Task {taskMap.TaskName} created");

				return Ok("New task created");
            }
            catch(Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            };
            
        }

        [HttpPut("{taskId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateTask(int taskId, [FromBody] TaskDto updateTask)
        {
			_logger.LogDebug($"UpdateTask taskId: {taskId}, TaskTDO: {JsonConvert.SerializeObject(updateTask)} started.");

            if (updateTask == null)
            {
				_logger.LogError($"400. Bad Request. {ModelState}");
				return BadRequest(ModelState);
            }

            if (taskId != updateTask.Id)
            {
				_logger.LogError($"400. Bad Request. {ModelState}");
				return BadRequest(ModelState);
            }
            
            try
            {
                var taskExist = await _taskService.IsTaskExist(updateTask.Id);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				if (!taskExist)
                {
					_logger.LogError($"404. Task {taskId} couldn't found");
					return NotFound();
                }

                var taskMap = _mapper.Map<Models.Task>(updateTask);

                taskMap.UpdateDate = DateTime.Now;
                taskMap.UpdateUserId = Enum.TEMP_USER_ID;
                taskMap.StatusId = (int)Enum.Status.Active;


                var updatedTask = await _taskService.UpdateTask(taskMap);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				_logger.LogInfo($"204. Task {taskId} updated.");
				return NoContent();
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }
            
        }

        [HttpDelete("{taskId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> DeleteTask(int taskId)
        {
			_logger.LogDebug($"DeleteTask taskID: {taskId} started");

            try
            {
                var taskToDelete = await _taskService.GetTaskById(taskId);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				if (taskToDelete == null)
                {
					_logger.LogError($"404. Task {taskId} couldn't found");
					return NotFound();
                }

                taskToDelete.StatusId = (int)Enum.Status.NotActive;
                taskToDelete.UpdateDate = DateTime.Now;
                taskToDelete.UpdateUserId = (int)Enum.TEMP_USER_ID;

                var taskToCompetitions = await _taskToCompetitionService.GetTaskToCompetitionByTaskId(taskId);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				if (taskToCompetitions.Count > 0)
                {
                    foreach (var taskToCompetition in taskToCompetitions)
                    {
                        taskToCompetition.UpdateDate = DateTime.Now;
                        taskToCompetition.UpdateUserId = (int)Enum.TEMP_USER_ID;
                        taskToCompetition.StatusId = (int)Enum.Status.NotActive;
                    }

                    await _taskToCompetitionService.DeleteTaskToCompetition(taskToCompetitions);
					
					if (!ModelState.IsValid)
					{
						_logger.LogError($"400. Bad Request. {ModelState}");
					}
				}

                await _taskService.DeleteTask(taskToDelete);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				_logger.LogInfo($"204. Task deleted {taskToDelete}");
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
