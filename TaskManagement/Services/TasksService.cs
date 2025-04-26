using System.Linq.Expressions;
using TaskManagement.Data;
using TaskManagement.DTOs;
using TaskManagement.Enums;
using TaskManagement.Models;
using TaskManagement.Repositories;

namespace TaskManagement.Services
{
    public class TasksService : ITasksService
    {
        private readonly ITasksRepository _tasksRepository;

        public TasksService(ITasksRepository tasksRepository)
        {
            _tasksRepository  = tasksRepository;
        }

        // Get a task by its ID

        public async Task<TaskItem> GetTaskByIdAsync(Guid taskId)
        {
            TaskItem task = await _tasksRepository.GetTaskByIdAsync(taskId);

            if (task == null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            return task;
           
        }

        // Get all the tasks reported by a specific user

        public async Task<IEnumerable<TaskItem>> GetTasksUserAsync(Guid userId)
        {
            return await _tasksRepository.GetTasksUserAsync(userId);
        }

        // Create a new task

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            return await _tasksRepository.CreateTaskAsync(task);  
        }

        // Update an existing task

        public async Task<TaskItem> UpdateTaskAsync(Guid taskId, TaskItem task)
        {
            TaskItem existingTask = await _tasksRepository.GetTaskByIdAsync(taskId);

            if(existingTask == null)
            {
                throw new KeyNotFoundException("Task not found");
            }
            
            return await _tasksRepository.UpdateTaskAsync(taskId, task);
            
        }

        // Delete a task

        public async Task<bool> DeleteTaskAsync(Guid taskId)
        {

            TaskItem existingTask = await _tasksRepository.GetTaskByIdAsync(taskId);

            if (existingTask == null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            return await _tasksRepository.DeleteTaskAsync(taskId);
          
        }

        // Share a task with another user
        public async Task ShareTaskUserAsync(Guid taskId, UserPermissionDTO participant)
        {
            TaskItem existingTask = await _tasksRepository.GetTaskByIdAsync(taskId);

            if (existingTask == null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            await _tasksRepository.ShareTaskUserAsync(taskId, participant.UserId, participant.Permission);
        
        }

        // Share a task with multiple users
        public async Task ShareTaskMultipleUsersAsync(Guid taskId, List<UserPermissionDTO> participants)
        {
            TaskItem existingTask = await _tasksRepository.GetTaskByIdAsync(taskId);

            if (existingTask == null)
            {
                throw new KeyNotFoundException("Task not found");
            }
           
            // Share task with each user
            foreach(UserPermissionDTO participant in participants)
            {
                await _tasksRepository.ShareTaskUserAsync(taskId, participant.UserId, participant.Permission);
            }
            
        }

        // Check if the user has permission to edit the task
        public async Task<bool> UserPermissionEditTaskAsync(Guid taskId, Guid userId)
        {
            TaskItem existingTask = await _tasksRepository.GetTaskByIdAsync(taskId);

            if (existingTask == null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            return await _tasksRepository.GetUserPermissionEditTaskAsync(taskId, userId);
            
        }

    }
}
