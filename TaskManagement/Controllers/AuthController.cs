using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Net;
using TaskManagement.Data;
using TaskManagement.DTOs;
using TaskManagement.Enums;
using TaskManagement.Helpers;
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
                ApiResponse<string> badRequestResponse = new ApiResponse<string>((int)HttpStatusCode.BadRequest, "Invalid request data");
                return BadRequest(badRequestResponse);
            }

            // Check if the username already exists
            bool userExists = await _context.Users.AnyAsync(u => u.UserName == request.UserName);

            if (userExists)
            {
                ApiResponse<string> userExistResponse = new ApiResponse<string>((int)HttpStatusCode.BadRequest, "Username already exists");
                return BadRequest(userExistResponse);
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

            ApiResponse<object> successResponse = new ApiResponse<object>((int)HttpStatusCode.Created, "User registered successfully", new { userId = user.UserId });
            return CreatedAtAction(nameof(Register), new { userId = user.UserId }, successResponse);
            

        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            // Find the user by username
            User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);

            if (user == null)
            {
                ApiResponse<string> unauthorizedResponse = new ApiResponse<string>((int)HttpStatusCode.Unauthorized, "Invalid username or password");
                return Unauthorized(unauthorizedResponse);
            }

            // Check if the password matches the hashed password in the db
            var result = _hasher.VerifyHashedPassword(user, user.Password, request.Password);

            if (result != PasswordVerificationResult.Success)
            {
                ApiResponse<string> unauthorizedResponse = new ApiResponse<string>((int)HttpStatusCode.Unauthorized, "Invalid username or password");
                return Unauthorized(unauthorizedResponse);
            }

            // Return response with user info
            var response = new LoginResponseDTO
            {
                UserId = user.UserId,
                UserName = request.UserName,
                Role = user.Role.ToString(),
            };
            
            ApiResponse<LoginResponseDTO> successResponse = new ApiResponse<LoginResponseDTO>((int)HttpStatusCode.OK, "Login successful", response);
                
            return Ok(successResponse);

        }


    }
}
