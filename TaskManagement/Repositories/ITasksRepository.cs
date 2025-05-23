﻿using TaskManagement.Enums;
using TaskManagement.Models;

namespace TaskManagement.Repositories
{
    public interface ITasksRepository
    {   
        // Retrieve a task by its ID
        public Task<TaskItem> GetTaskByIdAsync(Guid id);
        
        // Retrieve all the tasks reported by specified user
        public Task<IEnumerable<TaskItem>> GetTasksUserAsync(Guid userId);
        
        // Create a new task instance
        public Task<TaskItem> CreateTaskAsync(TaskItem task);
        
        // Update an existing task using its ID
        public Task<TaskItem> UpdateTaskAsync(Guid id, TaskItem task);
        
        // Delete a task using its ID
        public Task<bool> DeleteTaskAsync(Guid id);
        
     
        

       
    }
}
