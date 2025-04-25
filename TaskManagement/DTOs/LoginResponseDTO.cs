namespace TaskManagement.DTOs
{
    public class LoginResponseDTO
    {
        public string Message { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
    }
}
