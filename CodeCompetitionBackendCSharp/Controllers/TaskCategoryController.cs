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
        public IActionResult GetTaskCategories()
        {
            var taskCategories = _mapper.Map<List<TaskCategoryDto>>(_taskCategoryRepository.GetTaskCategories());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(taskCategories);
        }

        [HttpGet("{Id}")]
        [ProducesResponseType(200, Type = typeof(TaskCategory))]
        [ProducesResponseType(400)]
        public IActionResult GetTaskCategory(int Id)
        {
            if(!_taskCategoryRepository.IsTaskCategoryExist(Id))
            {
                return NotFound();
            }

            var taskCategory = _mapper.Map<TaskCategoryDto>(_taskCategoryRepository.GetTaskCategory(Id));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(taskCategory);
        }
    }
}
