using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TaskManagement.DTOs;
using TaskManagement.Enums;
using TaskManagement.Models;
using TaskManagement.Repositories;

namespace TaskManagement.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly PasswordHasher<User> _hasher;
        private readonly IMapper _mapper;
        public UsersService(IUsersRepository usersRepository, IMapper mapper)
        {
            _usersRepository = usersRepository;
            _hasher = new PasswordHasher<User>();
            _mapper = mapper;
        }

        // Get a user by its ID
        public async Task<UserDTO> GetUserByIdAsync(Guid userId)
        {
            User user = await _usersRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return _mapper.Map<UserDTO>(user);

        }

        // Get all users
        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            IEnumerable<User> users = await _usersRepository.GetAllUsersAsync();

            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        // Add a new user
        public async Task<UserDTO> RegisterUserAsync(string userName, string password)
        {
            // Check if the username already exists
            User existingUser = await _usersRepository.GetUserByUsernameAsync(userName);

            if (existingUser != null)
            { 
                throw new Exception("Username already exists");
            }

            // Create a new User instance 
            User user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = userName,
                Role = Role.User
            };

            // Hash the user's password
            user.Password = _hasher.HashPassword(user, password);

            // Add user in db
            User addedUser = await _usersRepository.AddUserAsync(user);

            return _mapper.Map<UserDTO>(addedUser);
        }

        // Delete a user
        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            //Check if user already exists
            User existingUser = await _usersRepository.GetUserByIdAsync(userId);

            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return await _usersRepository.DeleteUserAsync(userId);

        }

    }
}
