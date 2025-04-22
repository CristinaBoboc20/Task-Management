using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskManagement.Models
{
    public class TaskUser
    {
        [Key]
        public Guid TaskUserId { get; set; }
        
        [Required]
        public Guid TaskId { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public DateTime SharedAt { get; set; } = DateTime.UtcNow;
        
        [JsonIgnore]
        public virtual TaskItem Task { get; set; }
        
        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
