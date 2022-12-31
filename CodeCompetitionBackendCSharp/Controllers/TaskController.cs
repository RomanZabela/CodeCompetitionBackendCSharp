using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CodeCompetitionBackendCSharp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskRepository;
        private readonly IMapper _mapper;

        public TaskController(ITaskService taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Models.Task>))]
        public IActionResult getTasks()
        {
            //var tasks = _mapper.Map<List<TaskDto>>(_taskRepository.GetTasks());
            var tasks = _taskRepository.GetTasks();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(tasks);
        }

        [HttpGet("{taskId}")]
        [ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
        public IActionResult getTaskById(int teamID)
        {
            if (!_taskRepository.IsTaskExist(teamID))
            {
                return NotFound();
            }

            var task = _mapper.Map<TaskDto>(_taskRepository.GetTaskById(teamID));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(task);
        }
    }
}
