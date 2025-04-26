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
            try
            {
                TaskItem task = await _tasksRepository.GetTaskByIdAsync(taskId);

                if (task == null)
                {
                    throw new Exception("Task not found");
                }

                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while retrieving the specified task", ex);
            }
        }

        // Get all the tasks reported by a specific user

        public async Task<IEnumerable<TaskItem>> GetTasksUserAsync(Guid userId)
        {
            try
            {
                return await _tasksRepository.GetTasksUserAsync(userId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while retrieving all the tasks created by the specified user", ex);
            }
        }

        // Create a new task

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            try
            {
                return await _tasksRepository.CreateTaskAsync(task);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while creating this task", ex);
            }
        }

        // Update an existing task

        public async Task<TaskItem> UpdateTaskAsync(Guid taskId, TaskItem task)
        {
            try
            {
                return await _tasksRepository.UpdateTaskAsync(taskId, task);
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while updating task", ex);
            }
        }

        // Delete a task

        public async Task<bool> DeleteTaskAsync(Guid taskId)
        {
            try
            {
                return await _tasksRepository.DeleteTaskAsync(taskId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while deleting task", ex);
            }
        
        }

        // Share a task with another user
        public async Task ShareTaskUserAsync(Guid taskId, UserPermissionDTO participant)
        {
            try
            {
                await _tasksRepository.ShareTaskUserAsync(taskId, participant.UserId, participant.Permission);
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while sharing the task with another user", ex);
            }
        }

        // Share a task with multiple users
        public async Task ShareTaskMultipleUsersAsync(Guid taskId, List<UserPermissionDTO> participants)
        {
            try
            {
                // Share task with each user
                foreach(UserPermissionDTO participant in participants)
                {
                    await _tasksRepository.ShareTaskUserAsync(taskId, participant.UserId, participant.Permission);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while sharing the task with multiple users");
            }
        }

        // Check if the user has permission to edit the task
        public async Task<bool> UserPermissionEditTaskAsync(Guid taskId, Guid userId)
        {
            try
            {
                return await _tasksRepository.GetUserPermissionEditTaskAsync(taskId, userId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while checking user permission to edit task", ex);
            }
        }

    }
}
