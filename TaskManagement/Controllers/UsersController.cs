using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Net;
using TaskManagement.DTOs;
using TaskManagement.Helpers;
using TaskManagement.Models;
using TaskManagement.Services;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
  
        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        //Get user by its ID
        //GET: api/Users/{userid}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById([FromRoute] Guid userId)
        {
            // Retrieve user by its ID
            UserDTO userDTO = await _usersService.GetUserByIdAsync(userId);

            ApiResponse<UserDTO> response = new ApiResponse<UserDTO>((int)HttpStatusCode.OK, "User retrieved successfully", userDTO);

            return Ok(response);
        }

        //Get all users
        //GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            // Retrieve all userrs
            IEnumerable<UserDTO> userDTOs = await _usersService.GetAllUsersAsync();

            ApiResponse<IEnumerable<UserDTO>> response = new ApiResponse<IEnumerable<UserDTO>>((int)HttpStatusCode.OK, "All users retrieved successfully", userDTOs);

            return Ok(response);
        }

        // Register a new user
        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDTO request)
        {
            // Check if the request model is valid
            if (!ModelState.IsValid)
            {
                ApiResponse<string> badRequestResponse = new ApiResponse<string>((int)HttpStatusCode.BadRequest, "Invalid request data");
                return BadRequest(badRequestResponse);
            }

            UserDTO userDTO = await _usersService.RegisterUserAsync(request.UserName, request.Password);

            ApiResponse<UserDTO> successResponse = new ApiResponse<UserDTO>((int)HttpStatusCode.Created, "User registered successfully", userDTO);
            return CreatedAtAction(nameof(GetUserById), new { userId = userDTO.UserId }, successResponse);
        }

        //Delete user by its ID 
        //DELETE: api/Users/{userId}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            await _usersService.DeleteUserAsync(userId);
            
            ApiResponse<string> response = new ApiResponse<string>((int)HttpStatusCode.OK, "User deleted successfully");

            return Ok(response);
        }


    }
}
