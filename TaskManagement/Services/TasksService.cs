﻿using AutoMapper;
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
        private readonly IMapper _mapper;

        public TasksService(ITasksRepository tasksRepository, IMapper mapper)
        {
            _tasksRepository  = tasksRepository;
            _mapper = mapper;
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

        public async Task<TaskItem> CreateTaskAsync(CreateUpdateTaskDTO taskDTO, Guid userId)
        {
            // Mapping CreateUpdateTaskDTO to TaskItem
            TaskItem task = _mapper.Map<TaskItem>(taskDTO);

            // Set the creator of the task as the current user
            task.ReporterId = userId;

            return await _tasksRepository.CreateTaskAsync(task);  
        }

        // Update an existing task

        public async Task<TaskItem> UpdateTaskAsync(Guid taskId, CreateUpdateTaskDTO taskDTO)
        {
            TaskItem existingTask = await _tasksRepository.GetTaskByIdAsync(taskId);

            if(existingTask == null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            // Mapping CreateUpdateTaskDTO to existing TaskItem
            _mapper.Map(taskDTO, existingTask);

            return await _tasksRepository.UpdateTaskAsync(taskId, existingTask);
            
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


    }
}
