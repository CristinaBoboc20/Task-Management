using TaskManagement.DTOs;
using TaskManagement.Models;
using TaskManagement.Repositories;

namespace TaskManagement.Services
{
    public class TaskSharingService : ITaskSharingService
    {
        private readonly ITaskSharingRepository _taskSharingRepository;
        private readonly ITasksRepository _tasksRepository;

        public TaskSharingService(ITaskSharingRepository taskSharingRepository, ITasksRepository tasksRepository)
        {
            _taskSharingRepository = taskSharingRepository; 
            _tasksRepository = tasksRepository;
        }


        // Share a task with another user
        public async Task ShareTaskUserAsync(Guid taskId, UserPermissionDTO participant)
        {
            TaskItem existingTask = await _tasksRepository.GetTaskByIdAsync(taskId);

            if (existingTask == null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            await _taskSharingRepository.ShareTaskUserAsync(taskId, participant.UserId, participant.Permission);

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
            foreach (UserPermissionDTO participant in participants)
            {
                await _taskSharingRepository.ShareTaskUserAsync(taskId, participant.UserId, participant.Permission);
            }

        }

        // Check if the user has permission to edit the task
        public async Task<bool> GetParticipantPermissionEditTaskAsync(Guid taskId, Guid userId)
        {
            TaskItem existingTask = await _tasksRepository.GetTaskByIdAsync(taskId);

            if (existingTask == null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            return await _taskSharingRepository.GetParticipantPermissionEditTaskAsync(taskId, userId);

        }

    
    }
}
