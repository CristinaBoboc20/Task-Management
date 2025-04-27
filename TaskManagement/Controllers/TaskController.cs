using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Security.Claims;
using TaskManagement.DTOs;
using TaskManagement.Enums;
using TaskManagement.Helpers;
using TaskManagement.Models;
using TaskManagement.Services;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly IUserContextService _userContextService;
        private readonly ITasksService _tasksService;
        private readonly ITaskSharingService _tasksSharingService;
        private readonly IMapper _mapper;
       
        public TaskController(IUserContextService userContextService, ITasksService tasksService, ITaskSharingService taskSharingService, IMapper mapper)
        {
            _userContextService = userContextService;
            _tasksService = tasksService;
            _tasksSharingService = taskSharingService;
            _mapper = mapper;
        }

        // Get task by its ID
        // GET: api/Task/{taskId}
        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskById([FromRoute] Guid taskId)
        {
            
            // Retrieve task using its ID
            TaskItem task = await _tasksService.GetTaskByIdAsync(taskId);

            Guid userId = _userContextService.GetUserId();
            bool admin = _userContextService.IsAdmin();


            // Allow only the task creator, a participant or admin to access the task
            if (task.ReporterId != userId && !task.Participants.Any(p => p.UserId == userId) && !admin)
            {
                throw new UnauthorizedAccessException("You don't have permission");
            }

            ApiResponse<TaskItem> response = new ApiResponse<TaskItem>((int)HttpStatusCode.OK, "Task retrieved successfully", task);

            return Ok(response);

            

        }

        // Get all tasks created by the current user
        // GET: api/Task
        [HttpGet]
        public async Task<IActionResult> GetUserTasks()
        { 
            // Get the current user's ID
            Guid userId = _userContextService.GetUserId();

            //Retrieve all the tasks created by the current user
            IEnumerable<TaskItem> tasks = await _tasksService.GetTasksUserAsync(userId);

            ApiResponse<IEnumerable<TaskItem>> response = new ApiResponse<IEnumerable<TaskItem>>((int)HttpStatusCode.OK, "Task retrieved successfully", tasks);
            return Ok(response);

        }

        //Create a new task
        // POST: api/Task
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateUpdateTaskDTO taskDTO)
        {

            // Mapping CreateUpdateTaskDTO to TaskItem
            TaskItem task = _mapper.Map<TaskItem>(taskDTO);

            // Set the creator of the task as the current user
            task.ReporterId = _userContextService.GetUserId();
                 
            TaskItem createdTask = await _tasksService.CreateTaskAsync(task);

            ApiResponse<TaskItem> response = new ApiResponse<TaskItem>((int)HttpStatusCode.Created, "Task created successfully", createdTask);
            
            return CreatedAtAction(nameof(GetTaskById), new { taskId = createdTask.TaskId }, response);


        }

        // Update an existing task
        //PUT: api/Task/{taskId}
        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask([FromRoute] Guid taskId, [FromBody] CreateUpdateTaskDTO taskDTO)
        {
           
            // Get the existing Task
            TaskItem existingTask = await _tasksService.GetTaskByIdAsync(taskId);

            Guid userId = _userContextService.GetUserId();
                
            bool admin = _userContextService.IsAdmin();

            bool participantEditPermission = await _tasksSharingService.GetParticipantPermissionEditTaskAsync(taskId, userId);

            bool hasEditPermission = existingTask.ReporterId == userId || admin || participantEditPermission;
                
            // Only the creator, an admin or a participant that has write permission can update the task
            if (!hasEditPermission)
            {
                throw new UnauthorizedAccessException("You don't have permission..");
            }

            // Mapping CreateUpdateTaskDTO to existing TaskItem
            _mapper.Map(taskDTO, existingTask);

            // Update the task with the new data
            TaskItem updatedTask = await _tasksService.UpdateTaskAsync(taskId, existingTask);

            ApiResponse<TaskItem> response = new ApiResponse<TaskItem>((int)HttpStatusCode.OK, "Task updated successfully", updatedTask);

            return Ok(response);
          
        }

        // Delete a task
        // DELETE: api/Task/{taskId}
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask([FromRoute] Guid taskId)
        {
            // Get the existing task
            TaskItem task = await _tasksService.GetTaskByIdAsync(taskId);

            Guid userId = _userContextService.GetUserId();

            bool admin = _userContextService.IsAdmin();

            // Only the creator or an admin can delete the task
            if (task.ReporterId != userId && !admin)
            {
                throw new UnauthorizedAccessException("You don't have permission..");
            }

            await _tasksService.DeleteTaskAsync(taskId);

            ApiResponse<string> response = new ApiResponse<string>((int)HttpStatusCode.OK, "Task was deleted successfully");

            return Ok(response);

        }
    }
}
