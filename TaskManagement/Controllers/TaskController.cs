using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Security.Claims;
using TaskManagement.DTOs;
using TaskManagement.Enums;
using TaskManagement.Models;
using TaskManagement.Services;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITasksService _tasksService;
        private readonly IMapper _mapper;
        public TaskController(ITasksService tasksService, IMapper mapper)
        {
            _tasksService = tasksService;
            _mapper = mapper;
        }

        // Get task by its ID
        // GET: api/Task/{taskId}
        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskById([FromRoute] Guid taskId)
        {
            try
            {
                // Retrieve task using its ID
                TaskItem task = await _tasksService.GetTaskByIdAsync(taskId);

                Guid userId = GetUserId();
                bool admin = IsAdmin();


                // Allow only the task creator, a participant or admin to access the task
                if (task.ReporterId != userId && !task.Participants.Any(p => p.UserId == userId) && !admin)
                {
                    return Forbid(); // Deny access
                }
                
                return Ok(task); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // Get all tasks created by the current user
        // GET: api/Task
        [HttpGet]
        public async Task<IActionResult> GetUserTasks()
        { 
            try
            {
                // Get the current user's ID
                Guid userId = GetUserId();

                //Retrieve all the tasks created by the current user
                IEnumerable<TaskItem> tasks = await _tasksService.GetTasksUserAsync(userId);
                
                return Ok(tasks);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Create a new task
        // POST: api/Task
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateUpdateTaskDTO taskDTO)
        {
            try
            {
                // Mapping CreateUpdateTaskDTO to TaskItem
                TaskItem task = _mapper.Map<TaskItem>(taskDTO);

                // Set the creator of the task as the current user
                task.ReporterId = GetUserId();
                 
                TaskItem createdTask = await _tasksService.CreateTaskAsync(task);

                return CreatedAtAction(nameof(GetTaskById), new { taskId = createdTask.TaskId }, createdTask);

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Update an existing task
        //PUT: api/Task/{taskId}
        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask([FromRoute] Guid taskId, [FromBody] CreateUpdateTaskDTO taskDTO)
        {
            try
            {
                // Get the existing Task
                TaskItem existingTask = await _tasksService.GetTaskByIdAsync(taskId);

                Guid userId = GetUserId();
                
                bool admin = IsAdmin();

                bool participantEditPermission = await _tasksService.UserPermissionEditTaskAsync(taskId, userId);

                bool hasEditPermission = existingTask.ReporterId == userId || admin || participantEditPermission;
                
                // Only the creator, an admin or a participant that has write permission can update the task
                if (!hasEditPermission)
                {
                    return Forbid(); // Deny access
                }

                // Mapping CreateUpdateTaskDTO to existing TaskItem
                _mapper.Map(taskDTO, existingTask);

                // Update the task with the new data
                TaskItem updatedTask = await _tasksService.UpdateTaskAsync(taskId, existingTask);

                return Ok(updatedTask);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Delete a task
        // DELETE: api/Task/{taskId}
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask([FromRoute] Guid taskId)
        {
            try
            {
                // Get the existing task
                TaskItem task = await _tasksService.GetTaskByIdAsync(taskId);

                Guid userId = GetUserId();

                bool admin = IsAdmin();

                // Only the creator or an admin can delete the task
                if (task.ReporterId != userId && !admin)
                {
                    return Forbid(); // Deny access
                }

                await _tasksService.DeleteTaskAsync(taskId);

                return Ok("Task was successfully deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: apo/Task/{taskId}/share/participant
        [HttpPost("{taskId}/share/participant")]
        public async Task<IActionResult> ShareTask([FromRoute] Guid taskId, [FromBody] UserPermissionDTO participant)
        {
            try
            {
                // Get the existing task
                TaskItem task = await _tasksService.GetTaskByIdAsync(taskId);

                Guid userId = GetUserId();

                bool admin = IsAdmin();

                //Only the creator or an admin can share the task
                if (task.ReporterId != userId && !admin)
                {
                    return Forbid(); // Deny access
                }

                await _tasksService.ShareTaskUserAsync(taskId, participant);
                return Ok("Task was shared successfully");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Task/{taskId}/share/participants
        [HttpPost("{taskId}/share/participants")]
        public async Task<IActionResult> ShareTaskMultipleUsers([FromRoute] Guid taskId, [FromBody] List<UserPermissionDTO> participants)
        {
            try
            {
                // Retrieve task by its ID
                TaskItem task = await _tasksService.GetTaskByIdAsync(taskId);

                Guid userId = GetUserId();
                bool admin = IsAdmin();

                //Only the creator or an admin can share the task
                if (task.ReporterId != userId && !admin)
                {
                    return Forbid();
                }

                await _tasksService.ShareTaskMultipleUsersAsync(taskId, participants);

                return Ok("Task was shared successfully with selected users");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Retrieve the user's id from claims
        private Guid GetUserId()
        {
            string userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID claim is missing"); 
            }

            return Guid.Parse(userIdClaim);

        }
        

        // Retrieve the user's role from claims
        private string GetUserRole()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            return roleClaim?.Value ?? "User";
        } 
           
        
        // Check if the user is an admin
        private bool IsAdmin()
        {
            return string.Equals(GetUserRole(), "Admin", StringComparison.OrdinalIgnoreCase);
        } 
    }
}
