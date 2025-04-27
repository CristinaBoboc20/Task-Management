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
        public Task<TaskItem> CreateTaskAsync(CreateUpdateTaskDTO taskDTO, Guid userId);
        
        // Update the specified task with new values
        public Task<TaskItem> UpdateTaskAsync(Guid taskId, CreateUpdateTaskDTO taskDTO);
        
        // Delete the specified task
        public Task<bool> DeleteTaskAsync(Guid taskId);

        
    }
}
