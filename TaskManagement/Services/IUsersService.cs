using TaskManagement.DTOs;
using TaskManagement.Models;

namespace TaskManagement.Services
{
    public interface IUsersService
    {
        public Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        public Task<UserDTO> GetUserByIdAsync(Guid userId);
        public Task<UserDTO> RegisterUserAsync(string userName, string password);
        public Task<bool> DeleteUserAsync(Guid userId);
    }
}
