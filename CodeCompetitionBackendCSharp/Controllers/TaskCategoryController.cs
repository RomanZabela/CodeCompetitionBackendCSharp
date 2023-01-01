using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeCompetitionBackendCSharp.Controllers
{
    [Route("[controller]")]
    [ApiController]
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
        public IActionResult GetTaskCategories()
        {
            var taskCategories = _mapper.Map<List<TaskCategoryDto>>(_taskCategoryService.GetTaskCategories());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(taskCategories);
        }

        [HttpGet("{taskCategoryId}")]
        [ProducesResponseType(200, Type = typeof(TaskCategory))]
        [ProducesResponseType(400)]
        public IActionResult GetTaskCategory(int taskCategoryId)
        {
            if(!_taskCategoryService.IsTaskCategoryExist(taskCategoryId))
            {
                return NotFound();
            }

            var taskCategory = _mapper.Map<TaskCategoryDto>(_taskCategoryService.GetTaskCategory(taskCategoryId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(taskCategory);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult AddNewTaskCategory([FromBody] TaskCategoryDto taskCategory)
        {
            if (taskCategory == null)
            {
                return BadRequest(ModelState);
            }

            var taskCategoryExist = _taskCategoryService.GetTaskCategories()
                .Where(tc => tc.CategoryName.Trim().ToLower() == taskCategory.CategoryName.Trim().ToLower())
                .FirstOrDefault();

            if (taskCategoryExist != null)
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

            if (!_taskCategoryService.AddNewTaskCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong in saving process");
                return StatusCode(500, ModelState);
            }

            return Ok("Task category created");
        }

        [HttpPut("{taskCategoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public IActionResult UpdateTaskCategory(int taskCategoryId, [FromBody] TaskCategoryDto updateTaskCategory)
        {
            if (updateTaskCategory == null)
            {
                return BadRequest(ModelState);
            }

            if (taskCategoryId != updateTaskCategory.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_taskCategoryService.IsTaskCategoryExist(updateTaskCategory.Id))
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
            

            if (!_taskCategoryService.UpdateTaskCategory(taskCategoryMap))
            {
                ModelState.AddModelError("", "Failed update Task Category");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{taskCategoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteTaskCategory(int taskCategoryId)
        {

            var taskCategoryToDelete = _taskCategoryService.GetTaskCategory(taskCategoryId);
            
            if (_taskService.GetTasksByTaskCategory(taskCategoryId).Count() != 0)
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

            if(!_taskCategoryService.DeleteTaskCategory(taskCategoryToDelete))
            {
                ModelState.AddModelError("", "Failed to delete task category");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
