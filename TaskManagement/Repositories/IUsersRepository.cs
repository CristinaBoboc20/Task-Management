using TaskManagement.Models;

namespace TaskManagement.Repositories
{
    public interface IUsersRepository
    {
        public Task<IEnumerable<User>> GetAllUsersAsync();
        public Task<User> GetUserByIdAsync(Guid userId);
        public Task<User> AddUserAsync(User user);
        public Task<User> GetUserByUsernameAsync(string userName);
        public Task<bool> DeleteUserAsync(Guid userId);

    }
}
