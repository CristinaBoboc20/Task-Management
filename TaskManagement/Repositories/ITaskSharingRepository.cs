using TaskManagement.Enums;
using TaskManagement.Models;

namespace TaskManagement.Repositories
{
    public interface ITaskSharingRepository
    {
        // Share a task with another user
        public Task ShareTaskUserAsync(Guid taskId, Guid userId, Permission permission);

        //Check if the participant has write permission
        public Task<bool> GetParticipantPermissionEditTaskAsync(Guid taskId, Guid userId);



    }
}
