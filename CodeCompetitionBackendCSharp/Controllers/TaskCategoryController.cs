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

        public TaskCategoryController(ITaskCategoryService taskCategoryService, ITaskService taskService, IMapper mapper)
        {
            _taskCategoryService = taskCategoryService;
            _taskService = taskService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TaskCategory>))]
        public async Task<IActionResult> GetTaskCategories([FromQuery] TaskCategoryParameters taskCategoryParameters)
        {
            try
            {
                taskCategoryParameters.switchOff = taskCategoryParameters.switchOffString == "1" ? true : false;
                var getTaskResult = await _taskCategoryService.GetTaskCategories(taskCategoryParameters);
                
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(getTaskResult.MetaData));
                return Ok(getTaskResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpGet("{taskCategoryId}")]
        [ProducesResponseType(200, Type = typeof(TaskCategory))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTaskCategory(int taskCategoryId)
        {
            try
            {
                var getTaskResult = await _taskCategoryService.IsTaskCategoryExist(taskCategoryId);
                if (!getTaskResult)
                {
                    return NotFound();
                }

                var getTaskCategoryResult = await _taskCategoryService.GetTaskCategory(taskCategoryId);
                var taskCategory = _mapper.Map<TaskCategoryDto>(getTaskCategoryResult);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return Ok(taskCategory);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddNewTaskCategory([FromBody] TaskCategoryDto taskCategory)
        {
            if (taskCategory == null)
            {
                return BadRequest(ModelState);
            }

            TaskCategoryParameters taskCategoryParameters = new TaskCategoryParameters
            {
                switchOffString = "1",
                switchOff = true
            };

            var getTaskCategory = (await _taskCategoryService.GetTaskCategories(taskCategoryParameters))
                .Where(tc => tc.CategoryName.Trim().ToLower() == taskCategory.CategoryName.Trim().ToLower()).ToList(); 
            //var getTaskCategory = checkPagingResponse.Items.Where(tc => tc.CategoryName.Trim().ToLower() == taskCategory.CategoryName.Trim().ToLower());


            if (getTaskCategory.Count > 0)
            {
                ModelState.AddModelError("", "Task category already exist in DB");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
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

                return Ok("Task category created");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPut("{taskCategoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateTaskCategory(int taskCategoryId, [FromBody] TaskCategoryDto updateTaskCategory)
        {
            if (updateTaskCategory == null)
            {
                return BadRequest(ModelState);
            }

            if (taskCategoryId != updateTaskCategory.Id)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var isTaskCategoryExist = await _taskCategoryService.IsTaskCategoryExist(updateTaskCategory.Id);

                if (!isTaskCategoryExist)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var taskCategoryMap = _mapper.Map<TaskCategory>(updateTaskCategory);

                taskCategoryMap.UpdateDate = DateTime.Now;
                taskCategoryMap.UpdateUserId = Enum.TEMP_USER_ID;

                var updateTaskCategoryResult = await _taskCategoryService.UpdateTaskCategory(taskCategoryMap);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpDelete("{taskCategoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTaskCategory(int taskCategoryId)
        {

            try
            {
                var taskCategoryToDelete = await _taskCategoryService.GetTaskCategory(taskCategoryId);
                var getTaskByTaskCategory = await _taskService.GetTasksByTaskCategory(taskCategoryId);

                if (getTaskByTaskCategory.Count > 0)
                {
                    ModelState.AddModelError("", "Couldn't delete Task Category. Some tasks use this category");
                    return StatusCode(500, ModelState);
                };

                if (taskCategoryToDelete == null)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                taskCategoryToDelete.StatusId = (int)Enum.Status.NotActive;
                taskCategoryToDelete.UpdateDate = DateTime.Now;
                taskCategoryToDelete.UpdateUserId = Enum.TEMP_USER_ID;

                var deleteTaskCategoryResult = await _taskCategoryService.DeleteTaskCategory(taskCategoryToDelete);

                return NoContent();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }            
        }
    }
}
