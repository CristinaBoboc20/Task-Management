using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Enums;
using TaskManagement.Models;

namespace TaskManagement.Repositories
{
    public class TaskSharingRepository : ITaskSharingRepository
    {
        private readonly ApplicationDbContext _context;
        public TaskSharingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Share a task with another user
        public async Task ShareTaskUserAsync(Guid taskId, Guid userId, Permission permission)
        {
            // Get the task by its ID from the db
            TaskItem taskDb = await _context.TaskItems.FindAsync(taskId);

            // Check if the task exists
            if (taskDb == null)
            {
                throw new Exception("Task not found in db");
            }

            // Check it's already shared with the given user
            TaskUser taskUserDb = await _context.TaskUsers.FirstOrDefaultAsync(tu => tu.TaskId == taskId && tu.UserId == userId);

            if (taskUserDb != null)
            {
                taskUserDb.Permission = permission;
            }
            else
            {
                // Initialize a new shared task instance
                TaskUser taskUser = new TaskUser
                {
                    TaskUserId = Guid.NewGuid(),
                    TaskId = taskId,
                    UserId = userId,
                    SharedAt = DateTime.UtcNow,
                    Permission = permission
                };

                // Add the shared task to the context
                _context.TaskUsers.Add(taskUser);
            }
            // Save changes to the db
            await _context.SaveChangesAsync();
        }


        // Check if the participant has write permissiom
        public async Task<bool> GetParticipantPermissionEditTaskAsync(Guid taskId, Guid userId)
        {
            // Get the shared task 
            TaskUser taskUser = await _context.TaskUsers.Where(tu => tu.TaskId == taskId && tu.UserId == userId)
                                                        .FirstOrDefaultAsync();
            if (taskUser == null)
            {
                return false;
            }

            return taskUser.Permission == Permission.ReadWrite;

        }

       

       
    }
}
