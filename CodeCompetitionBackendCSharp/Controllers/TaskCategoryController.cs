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
    [Authorize]
    public class TaskCategoryController : Controller
    {
        private readonly ITaskCategoryService _taskCategoryService;
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;
		private readonly ILoggerManager _logger;

		public TaskCategoryController(ITaskCategoryService taskCategoryService,
										ITaskService taskService,
										IMapper mapper,
										ILoggerManager logger)
		{
			_taskCategoryService = taskCategoryService;
			_taskService = taskService;
			_mapper = mapper;
			_logger = logger;
		}

		[HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TaskCategory>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> GetTaskCategories([FromQuery] TaskCategoryParameters taskCategoryParameters)
        {
            _logger.LogDebug($"GetTaskCategories {JsonConvert.SerializeObject(taskCategoryParameters)} started");

            try
            {
                taskCategoryParameters.switchOff = taskCategoryParameters.switchOffString == "1" ? true : false;
                var getTaskResult = await _taskCategoryService.GetTaskCategories(taskCategoryParameters);
                
                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
                }

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(getTaskResult.MetaData));

				_logger.LogInfo($"200. All TaskCategories: {JsonConvert.SerializeObject(getTaskResult)}");
				return Ok(getTaskResult);
            }
            catch (Exception ex)
            {
				_logger.LogError(ex.Message);
				return StatusCode(500, ex.Message);
            }
            
        }

        [HttpGet("{taskCategoryId}")]
        [ProducesResponseType(200, Type = typeof(TaskCategory))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> GetTaskCategory(int taskCategoryId)
        {
            _logger.LogDebug($"GetTaskCategory {taskCategoryId} started");

            try
            {
                var getTaskResult = await _taskCategoryService.IsTaskCategoryExist(taskCategoryId);
                if (!getTaskResult)
                {
					_logger.LogError($"404. Not found Task category with id {taskCategoryId}");
					return NotFound();
                }

                var getTaskCategoryResult = await _taskCategoryService.GetTaskCategory(taskCategoryId);
                var taskCategory = _mapper.Map<TaskCategoryDto>(getTaskCategoryResult);

                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
                }

				_logger.LogInfo($"200. TaskCategory: {taskCategory.Id}");
				return Ok(taskCategory);
            }
            catch(Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }
            
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(422)]
		public async Task<IActionResult> AddNewTaskCategory([FromBody] TaskCategoryDto taskCategory)
        {
            _logger.LogDebug($"AddNewTaskCategory {JsonConvert.SerializeObject(taskCategory)}");
            if (taskCategory == null)
            {
				_logger.LogError($"400. Bad Request. {ModelState}");
				return BadRequest(ModelState);
            }

            TaskCategoryParameters taskCategoryParameters = new TaskCategoryParameters
            {
                switchOffString = "1",
                switchOff = true
            };

            try
            {
				var getTaskCategory = (await _taskCategoryService.GetTaskCategories(taskCategoryParameters))
				                .Where(tc => tc.CategoryName.Trim().ToLower() == taskCategory.CategoryName.Trim().ToLower()).ToList();

				if (getTaskCategory.Count > 0)
				{
					_logger.LogError($"409. Task category {taskCategory.Id} already exist in DB");
					return Conflict("Task category already exist in DB");
				}
			}
            catch (Exception error)
            {
                _logger.LogInfo($"500. Couldn't find {taskCategory.Id}, got an error: {error.Message}");
            }

            if (!ModelState.IsValid)
            {
				_logger.LogError($"400. Bad Request. {ModelState}");
				return BadRequest(ModelState);
            }

            var categoryMap = _mapper.Map<TaskCategory>(taskCategory);

            categoryMap.UpdateDate = DateTime.Now;
            categoryMap.CreateDate = DateTime.Now;
            categoryMap.UpdateUserId = Enum.TEMP_USER_ID;
            categoryMap.CreateUserId = Enum.TEMP_USER_ID;
            categoryMap.StatusId = (int)Enum.Status.Active;

            try
            {
                var taskAddedResult = await _taskCategoryService.AddNewTaskCategory(categoryMap);

				_logger.LogInfo($"200. Task category created");
				return Ok("Task category created");
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }

        }

        [HttpPut("{taskCategoryId}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> UpdateTaskCategory(int taskCategoryId, [FromBody] TaskCategoryDto updateTaskCategory)
        {
            _logger.LogDebug($"UpdateTaskCategory taskCategoryId:{taskCategoryId} TaskCategoryDto: {JsonConvert.SerializeObject(updateTaskCategory)} started");

            if (updateTaskCategory == null)
            {
				_logger.LogError($"400. Bad Request. {ModelState}");
				return BadRequest(ModelState);
            }

            if (taskCategoryId != updateTaskCategory.Id)
            {
				_logger.LogError($"400. Bad Request. {ModelState}");
				return BadRequest(ModelState);
            }

            try
            {
                var isTaskCategoryExist = await _taskCategoryService.IsTaskCategoryExist(updateTaskCategory.Id);

                if (!isTaskCategoryExist)
                {
					_logger.LogError($"404. Not Found TaskCategory {updateTaskCategory.Id}");
					return NotFound();
                }

                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
                }

                var taskCategoryMap = _mapper.Map<TaskCategory>(updateTaskCategory);

                taskCategoryMap.UpdateDate = DateTime.Now;
                taskCategoryMap.UpdateUserId = Enum.TEMP_USER_ID;

                var updateTaskCategoryResult = await _taskCategoryService.UpdateTaskCategory(taskCategoryMap);

				_logger.LogInfo($"204. TaskCategory {taskCategoryId} updated");
				return NoContent();
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }
            
        }

        [HttpDelete("{taskCategoryId}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> DeleteTaskCategory(int taskCategoryId)
        {
            _logger.LogDebug($"DeleteTaskCategory taskCategoryId:{taskCategoryId} started.");

            try
            {
                var taskCategoryToDelete = await _taskCategoryService.GetTaskCategory(taskCategoryId);
                var getTaskByTaskCategory = await _taskService.GetTasksByTaskCategory(taskCategoryId);

                if (getTaskByTaskCategory.Count > 0)
                {
					_logger.LogError("409. Couldn't delete Task Category. Some tasks use this category");
					return Conflict("Couldn't delete Task Category. Some tasks use this category");
				};

                if (taskCategoryToDelete == null)
                {
					_logger.LogError($"404. Task Category {taskCategoryId} not found");
					return NotFound();
                }

                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
                }

                taskCategoryToDelete.StatusId = (int)Enum.Status.NotActive;
                taskCategoryToDelete.UpdateDate = DateTime.Now;
                taskCategoryToDelete.UpdateUserId = Enum.TEMP_USER_ID;

                var deleteTaskCategoryResult = await _taskCategoryService.DeleteTaskCategory(taskCategoryToDelete);
				
                _logger.LogInfo($"204. Task Category {taskCategoryId} deleted.");
				return NoContent();
            }
            catch(Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }            
        }
    }
}
