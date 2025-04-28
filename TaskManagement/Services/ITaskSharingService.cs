using TaskManagement.DTOs;
using TaskManagement.Models;

namespace TaskManagement.Services
{
    public interface ITaskSharingService
    {
        // Share a task with another user
        public Task ShareTaskUserAsync(Guid taskId, UserPermissionDTO participant);

        // Share a task with multiple users
        public Task ShareTaskMultipleUsersAsync(Guid taskId, List<UserPermissionDTO> participants);

        // Check if the user has permission to edit the task
        public Task<bool> GetParticipantPermissionEditTaskAsync(Guid taskId, Guid userId);

       
    }
}
