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
        public async Task<IActionResult> GetTasks()
        {
            var taskAsync = await _taskService.GetTasks();
            var tasks = _mapper.Map<List<TaskDto>>(taskAsync);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(tasks);
        }

        [HttpGet("{taskId}")]
        [ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTaskById(int taskId)
        {
            var taskExist = await _taskService.IsTaskExist(taskId);

            if (!taskExist)
            {
                return NotFound();
            }

            var taskAsync = await _taskService.GetTaskById(taskId);
            var task = _mapper.Map<TaskDto>(taskAsync);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(task);
        }

        [HttpGet("{taskCategoryId}/Tasks")]
        [ProducesResponseType(200, Type = typeof(Models.Task))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTaskByTaskCategory(int taskCategoryId)
        {
            var taskByCategory = await _taskService.GetTasksByTaskCategory(taskCategoryId);

            if (taskByCategory == null)
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
        public async Task<IActionResult> AddNewTask([FromBody] TaskDto task)
        {
            if (task == null)
            {
                return BadRequest(ModelState);
            }

            var taskExist = await _taskService.GetTasks();
            var sameTaskName = taskExist.Where(te => te.TaskName.Trim().ToLower() == task.TaskName.Trim().ToLower());

            if (sameTaskName != null)
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

            if (!newTask)
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

            var updatedTask = await _taskService.UpdateTask(taskMap);

            if (!updatedTask)
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
        public async Task<IActionResult> DeleteTask(int taskId)
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

            var taskToConnections = await _taskToCompetitionService.GetTaskToCompetitionByTaskId(taskId);

            if (taskToConnections != null) 
            {
                foreach(var taskToCompetition in taskToConnections)
                {
                    taskToCompetition.UpdateDate = DateTime.Now;
                    taskToCompetition.UpdateUserId = (int)Enum.TEMP_USER_ID;
                    taskToCompetition.StatusId = (int)Enum.Status.NotActive;
                    
                    await _taskToCompetitionService.DeleteTaskToCompetition(taskToCompetition);
                }
            }

            var deleteResult = await _taskService.DeleteTask(taskToDelete);

            if (!deleteResult)
            {
                ModelState.AddModelError("", "Failed to delete task category");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
