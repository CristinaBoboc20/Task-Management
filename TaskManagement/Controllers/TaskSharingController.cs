using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using TaskManagement.DTOs;
using TaskManagement.Helpers;
using TaskManagement.Models;
using TaskManagement.Services;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskSharingController : ControllerBase
    {
        private readonly ITaskSharingService _tasksSharingService;
        private readonly ITasksService _tasksService;
        private readonly IMapper _mapper;

        public TaskSharingController(ITaskSharingService taskSharingService, ITasksService tasksService,IMapper mapper)
        {
            _tasksSharingService = taskSharingService;
            _tasksService = tasksService;
            _mapper = mapper;
        }


        // POST: api/TaskSharing/{taskId}/participant
        [HttpPost("{taskId}/participant")]
        public async Task<IActionResult> ShareTask([FromRoute] Guid taskId, [FromBody] UserPermissionDTO participant)
        {

            // Get the existing task
            TaskItem task = await _tasksService.GetTaskByIdAsync(taskId);

            Guid userId = GetUserId();

            bool admin = IsAdmin();

            //Only the creator or an admin can share the task
            if (task.ReporterId != userId && !admin)
            {
                throw new UnauthorizedAccessException("You don't have permission");
            }

            await _tasksSharingService.ShareTaskUserAsync(taskId, participant);

            ApiResponse<string> response = new ApiResponse<string>((int)HttpStatusCode.OK, "Task was shared successfully");

            return Ok(response);


        }

        // POST: api/TaskSharing/{taskId}/participants
        [HttpPost("{taskId}/share/participants")]
        public async Task<IActionResult> ShareTaskMultipleUsers([FromRoute] Guid taskId, [FromBody] List<UserPermissionDTO> participants)
        {
            // Retrieve task by its ID
            TaskItem task = await _tasksService.GetTaskByIdAsync(taskId);

            Guid userId = GetUserId();
            bool admin = IsAdmin();

            //Only the creator or an admin can share the task
            if (task.ReporterId != userId && !admin)
            {
                throw new UnauthorizedAccessException("You don't have permission");
            }

            await _tasksSharingService.ShareTaskMultipleUsersAsync(taskId, participants);

            ApiResponse<string> response = new ApiResponse<string>((int)HttpStatusCode.OK, "Task was shared successfully with selected users");

            return Ok(response);

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
