using TaskManagement.DTOs;
using TaskManagement.Enums;
using TaskManagement.Models;

namespace TaskManagement.Services
{
    public interface ITasksService
    {
        // Retrieve a task by its ID
        public Task<TaskItem> GetTaskByIdAsync(Guid taskId);

        //Retrieve all tasks created by specified user
        public Task<IEnumerable<TaskItem>> GetTasksUserAsync(Guid userId);
        
        // Create a new task
        public Task<TaskItem> CreateTaskAsync(TaskItem task);
        
        // Update the specified task with new values
        public Task<TaskItem> UpdateTaskAsync(Guid taskId, TaskItem task);
        
        // Delete the specified task
        public Task<bool> DeleteTaskAsync(Guid taskId);

        // Share a task with another user
        public Task ShareTaskUserAsync(Guid taskId, UserPermissionDTO participant);

        // Share a task with multiple users
        public Task ShareTaskMultipleUsersAsync(Guid taskId, List<UserPermissionDTO> participants);

        // Check if the user has permission to edit the task
        public Task<bool> UserPermissionEditTaskAsync(Guid taskId, Guid userId);
    }
}
