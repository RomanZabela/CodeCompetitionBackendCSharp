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
        public IActionResult GetTasks()
        {
            var tasks = _mapper.Map<List<TaskDto>>(_taskService.GetTasks());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(tasks);
        }

        [HttpGet("{taskId}")]
        [ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
        public IActionResult GetTaskById(int taskId)
        {
            if (!_taskService.IsTaskExist(taskId))
            {
                return NotFound();
            }

            var task = _mapper.Map<TaskDto>(_taskService.GetTaskById(taskId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(task);
        }

        [HttpGet("{taskCategoryId}/Tasks")]
        [ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
        public IActionResult GetTaskByTaskCategory(int taskCategoryId)
        {
            var taskByCategory = _taskService.GetTasksByTaskCategory(taskCategoryId);

            if (taskByCategory.Count() == 0)
            {
                return NotFound();
            }

            var taskMap = _mapper.Map<List<TaskDto>>(taskByCategory);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(taskMap);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult AddNewTask([FromBody] TaskDto task)
        {
            if (task == null)
            {
                return BadRequest(ModelState);
            }

            var taskExist = _taskService.GetTasks()
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

            taskMap.TaskCategory = _taskCategoryService.GetTaskCategory(task.TaskCategoryId);

            if (!_taskService.AddTask(taskMap))
            {
                ModelState.AddModelError("", "Something went wrong in saving process");
                return StatusCode(500, ModelState);
            }

            return Ok("Task category created");
        }

        [HttpPut("{taskId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public IActionResult UpdateTask(int taskId, [FromBody] TaskDto updateTask)
        {
            if (updateTask == null)
            {
                return BadRequest(ModelState);
            }

            if (taskId != updateTask.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_taskService.IsTaskExist(updateTask.Id))
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


            if (!_taskService.UpdateTask(taskMap))
            {
                ModelState.AddModelError("", "Failed update Task Category");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{taskId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteTask(int taskId)
        {

            var taskToDelete = _taskService.GetTaskById(taskId);

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

            var taskToConnection = _taskToCompetitionService.GetCompetitionIdsByTaskId(taskId);

            if (taskToConnection.Count() > 0) 
            {
                foreach(var taskToCompetition in taskToConnection)
                {
                    taskToCompetition.UpdateDate = DateTime.Now;
                    taskToCompetition.UpdateUserId = (int)Enum.TEMP_USER_ID;
                    taskToCompetition.StatusId = (int)Enum.Status.NotActive;
                    
                    _taskToCompetitionService.DeleteTaskToCompetitionConnection(taskToCompetition);
                }
            }

            if (!_taskService.DeleteTask(taskToDelete))
            {
                ModelState.AddModelError("", "Failed to delete task category");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
