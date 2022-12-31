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
        private readonly ITaskCategoryService _taskCategoryRepository;
        private readonly IMapper _mapper;

        public TaskCategoryController(ITaskCategoryService taskCategoryRepository, IMapper mapper)
        {
            _taskCategoryRepository = taskCategoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TaskCategory>))]
        public IActionResult getTaskCategories()
        {
            var taskCategories = _mapper.Map<List<TaskCategoryDto>>(_taskCategoryRepository.GetTaskCategories());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(taskCategories);
        }

        [HttpGet("{taskCategoryId}")]
        [ProducesResponseType(200, Type = typeof(TaskCategory))]
        [ProducesResponseType(400)]
        public IActionResult getTaskCategory(int taskCategoryId)
        {
            if(!_taskCategoryRepository.IsTaskCategoryExist(taskCategoryId))
            {
                return NotFound();
            }

            var taskCategory = _mapper.Map<TaskCategoryDto>(_taskCategoryRepository.GetTaskCategory(taskCategoryId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(taskCategory);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult addNewTaskCategory([FromBody] TaskCategoryDto taskCategory)
        {
            if (taskCategory == null)
            {
                return BadRequest(ModelState);
            }

            var taskCategoryExist = _taskCategoryRepository.GetTaskCategories()
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

            if (!_taskCategoryRepository.AddNewTaskCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong in saving process");
                return StatusCode(500, ModelState);
            }

            return Ok("Task category created");
        }
    }
}
