using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
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

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult addNewTask([FromBody] TaskDto task)
        {
            if (task == null)
            {
                return BadRequest(ModelState);
            }

            var taskExist = _taskRepository.GetTasks()
                .Where(t => t.TaskName.Trim().ToLower() == task.TaskName.Trim().ToLower())
                .FirstOrDefault();

            if (taskExist != null)
            {
                ModelState.AddModelError("", "Task already exist in DB");
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

            if (!_taskRepository.AddTask(taskMap))
            {
                ModelState.AddModelError("", "Something went wrong in saving process");
                return StatusCode(500, ModelState);
            }

            return Ok("Task category created");
        }
    }
}
