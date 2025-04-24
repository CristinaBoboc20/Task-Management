using System.ComponentModel.DataAnnotations;

namespace TaskManagement.DTOs
{
    public class RegisterRequestDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
