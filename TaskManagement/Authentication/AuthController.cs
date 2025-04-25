using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using TaskManagement.Data;
using TaskManagement.DTOs;
using TaskManagement.Enums;
using TaskManagement.Models;

namespace TaskManagement.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _hasher;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
            _hasher = new PasswordHasher<User>();
        }

        //POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            // Check if the request model is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                // Check if the username already exists
                bool userExists = await _context.Users.AnyAsync(u => u.UserName == request.UserName);

                if (userExists)
                {
                    return BadRequest("Username already exists");
                }

                // Create a new User instance 
                User user = new User
                {
                    UserId = Guid.NewGuid(),
                    UserName = request.UserName,
                    Role = Role.User
                };

                // Hash the user's password
                user.Password = _hasher.HashPassword(user, request.Password);

                //Add user to db
                _context.Users.Add(user);

                //Save changes 
                await _context.SaveChangesAsync();

                return Ok(new { message = "User registered successfully", userId = user.UserId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error occurred during registration");
            }
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {

            try
            {
                // Find the user by username
                User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);

                if (user == null)
                {
                    return Unauthorized("Invalid username or password");
                }


                // Check if the password matches the hashed password in the db
                var result = _hasher.VerifyHashedPassword(user, user.Password, request.Password);

                if (result != PasswordVerificationResult.Success)
                {
                    return Unauthorized("Invalid username or password");
                }

                // Return response with user info
                var response = new LoginResponseDTO
                {
                    Message = "Login successful",
                    UserId = user.UserId,
                    UserName = request.UserName,
                    Role = user.Role.ToString(),
                };

                return Ok(response);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "Error occurred during login");
            }
        }


    }
}
