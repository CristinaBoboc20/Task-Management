using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using TaskManagement.Data;
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
            try
            {
                // Return the task including the users it was shared with
                return await _context.TaskItems.Include(t => t.Participants).FirstOrDefaultAsync(t => t.TaskId == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving task", ex);
            }
        }

        // Retrieves all tasks created by a given user
        public async Task<IEnumerable<TaskItem>> GetTasksUserAsync(Guid userId)
        {
            try
            {
                // Return tasks where the reporter is the current user
                return await _context.TaskItems.Include(t => t.Participants).Where(t => t.ReporterId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving tasks for the given user from database", ex);
            }

        }

        // Create a new task
        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            try
            {
                // Generate a new GUID for the task ID
                task.TaskId = Guid.NewGuid();

                // Add the task to the context
                _context.TaskItems.Add(task);

                // Save changes to the db
                await _context.SaveChangesAsync();

                return task;
            }
            catch(Exception ex)
            {
                throw new Exception("Error creating task", ex);
            }
        }

        // Update a task with new values
        public async Task<TaskItem> UpdateTaskAsync(Guid id, TaskItem task)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("Error updating task from db", ex);
            }
        }

        // Delete a task by its ID
        public async Task<bool> DeleteTaskAsync(Guid id)
        {

            try
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
            catch (Exception ex)
            {
                throw new Exception("Error on deleting task", ex);
            }

        }

        // Share a task with another user
        public async Task ShareTaskUserAsync(Guid taskId, Guid userId)
        {
            try
            {
                // Get the task by its ID from the db
                TaskItem taskDb = await GetTaskByIdAsync(taskId);

                // Check if the task exists
                if(taskDb == null)
                {
                    throw new Exception("Task not found in db");
                }

                // Check it's already shared with the given user
                TaskUser taskUserDb = await _context.TaskUsers.FirstOrDefaultAsync(tu => tu.TaskId == taskId && tu.UserId == userId);

                if(taskUserDb != null)
                {
                    return;
                }

                // Initialize a new shared task instance
                TaskUser taskUser = new TaskUser
                {
                    TaskUserId = Guid.NewGuid(),
                    TaskId = taskId,
                    UserId = userId,
                    SharedAt = DateTime.UtcNow
                };

                // Add the shared task to the context
                _context.TaskUsers.Add(taskUser);

                // Save changes to the db
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Error sharing task with user", ex);
            }
        }



    }
}
