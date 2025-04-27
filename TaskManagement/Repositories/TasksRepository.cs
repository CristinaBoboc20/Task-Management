using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using TaskManagement.Data;
using TaskManagement.Enums;
using TaskManagement.Models;

namespace TaskManagement.Repositories
{
    public class TasksRepository : ITasksRepository
    {
        private readonly ApplicationDbContext _context;

        public TasksRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves a task by its ID
        public async Task<TaskItem> GetTaskByIdAsync(Guid id)
        {
            // Return the task including the users it was shared with
            return await _context.TaskItems.Include(t => t.Participants).FirstOrDefaultAsync(t => t.TaskId == id); 
        }

        // Retrieves all tasks created by a given user
        public async Task<IEnumerable<TaskItem>> GetTasksUserAsync(Guid userId)
        {
            // Return tasks where the reporter is the current user
            return await _context.TaskItems.Include(t => t.Participants).Where(t => t.ReporterId == userId).ToListAsync();
        }

        // Create a new task
        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            // Generate a new GUID for the task ID
            task.TaskId = Guid.NewGuid();

            // Add the task to the context
            _context.TaskItems.Add(task);

            // Save changes to the db
            await _context.SaveChangesAsync();

            return task;
        }

        // Update a task with new values
        public async Task<TaskItem> UpdateTaskAsync(Guid id, TaskItem task)
        {

            // Get the task by its ID from the db
            TaskItem taskDb = await GetTaskByIdAsync(id);
                
            if(taskDb == null)
            {
                throw new Exception("Task not found in db");
            }

            // Update task fields with new values
            taskDb.Title = task.Title;
            taskDb.Description = task.Description;
            taskDb.Status = task.Status;
            taskDb.Priority = task.Priority;
            taskDb.DueDate = task.DueDate;

            // Save changes to the db
            await _context.SaveChangesAsync();
                
            return taskDb;
          
        }

        // Delete a task by its ID
        public async Task<bool> DeleteTaskAsync(Guid id)
        {
            // Get the task by its ID from the db
            TaskItem taskDb = await GetTaskByIdAsync(id);

            if (taskDb == null)
            {
                throw new Exception("Task not found in database");
            }

            // Remove the task
            _context.TaskItems.Remove(taskDb);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
