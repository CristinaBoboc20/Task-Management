using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext _context;
        public UsersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves a user by its ID
        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }

        // Retrieves all users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Add a new user
        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);

            await _context.SaveChangesAsync();
            
            return user;
        }

        // Retrieve user by specified username
        public async Task<User> GetUserByUsernameAsync(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }

        // Delete a user by its ID
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            // Get the user by its ID from the db
            User user = await GetUserByIdAsync(id);

            // Remove the user
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
