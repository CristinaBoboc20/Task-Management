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
        private readonly IUserContextService _userContextService;
        private readonly ITaskSharingService _tasksSharingService;
        private readonly ITasksService _tasksService;
        private readonly IMapper _mapper;

        public TaskSharingController(IUserContextService userContextService,ITaskSharingService taskSharingService, ITasksService tasksService,IMapper mapper)
        {
            _userContextService = userContextService;
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

            Guid userId = _userContextService.GetUserId();

            bool admin = _userContextService.IsAdmin();

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
        [HttpPost("{taskId}/participants")]
        public async Task<IActionResult> ShareTaskMultipleUsers([FromRoute] Guid taskId, [FromBody] List<UserPermissionDTO> participants)
        {
            // Retrieve task by its ID
            TaskItem task = await _tasksService.GetTaskByIdAsync(taskId);

            Guid userId = _userContextService.GetUserId();
            bool admin = _userContextService.IsAdmin();

            //Only the creator or an admin can share the task
            if (task.ReporterId != userId && !admin)
            {
                throw new UnauthorizedAccessException("You don't have permission");
            }

            await _tasksSharingService.ShareTaskMultipleUsersAsync(taskId, participants);

            ApiResponse<string> response = new ApiResponse<string>((int)HttpStatusCode.OK, "Task was shared successfully with selected users");

            return Ok(response);

        }

    }
}
