using TaskManagement.Enums;

namespace TaskManagement.DTOs
{
    public class UserPermissionDTO
    {
        public Guid UserId { get; set; }
        public Permission Permission { get; set; }
    }
}
