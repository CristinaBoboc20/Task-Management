using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManagement.Enums;

namespace TaskManagement.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        public Role Role { get; set; } = Role.User;

        [JsonIgnore]
        public virtual ICollection<TaskItem> ReportedTasks { get; set; } = new List<TaskItem>();

        [JsonIgnore]
        public virtual ICollection<TaskUser> SharedTasks { get; set; } = new List<TaskUser>();

        
    }
}
