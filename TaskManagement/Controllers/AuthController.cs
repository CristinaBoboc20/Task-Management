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
using TaskManagement.Services;

namespace TaskManagement.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public AuthController(IUsersService usersService)
        {
            _usersService = usersService;
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

            UserDTO userDTO = await _usersService.RegisterUserAsync(request.UserName, request.Password);

            ApiResponse<object> successResponse = new ApiResponse<object>((int)HttpStatusCode.Created, "User registered successfully", userDTO);
            return CreatedAtAction(nameof(Register), new { userId = userDTO.UserId }, successResponse);


        }

        //// POST: api/Auth/login
        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        //{
        //    // Find the user by username
        //    User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);

        //    if (user == null)
        //    {
        //        ApiResponse<string> unauthorizedResponse = new ApiResponse<string>((int)HttpStatusCode.Unauthorized, "Invalid username or password");
        //        return Unauthorized(unauthorizedResponse);
        //    }

        //    // Check if the password matches the hashed password in the db
        //    var result = _hasher.VerifyHashedPassword(user, user.Password, request.Password);

        //    if (result != PasswordVerificationResult.Success)
        //    {
        //        ApiResponse<string> unauthorizedResponse = new ApiResponse<string>((int)HttpStatusCode.Unauthorized, "Invalid username or password");
        //        return Unauthorized(unauthorizedResponse);
        //    }

        //    // Return response with user info
        //    var response = new LoginResponseDTO
        //    {
        //        UserId = user.UserId,
        //        UserName = request.UserName,
        //        Role = user.Role.ToString(),
        //    };
            
        //    ApiResponse<LoginResponseDTO> successResponse = new ApiResponse<LoginResponseDTO>((int)HttpStatusCode.OK, "Login successful", response);
                
        //    return Ok(successResponse);

        //}


    }
}
