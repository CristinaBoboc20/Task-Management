using System.Security.Claims;
using TaskManagement.Models;

namespace TaskManagement.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Retrieve the user's id from claims
        public Guid GetUserId()
        {
            string userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID claim is missing");
            }

            return Guid.Parse(userIdClaim);

        }


        // Check if the user is an admin
        public bool IsAdmin()
        {

            // Retrieve the user's role from claims
            string role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? "User";
            
            return string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);
        }

    }
}
