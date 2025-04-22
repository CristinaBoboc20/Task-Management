using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManagement.Enums;

namespace TaskManagement.Models
{
    public class TaskItem
    {
        [Key]
        public Guid TaskId { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;

        public Status Status { get; set; } = Status.ToDo;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        
        [Required]
        public Guid ReporterId { get; set; }
        
        [JsonIgnore]
        public virtual User Reporter { get; set; }

        [JsonIgnore]
        public virtual ICollection<TaskUser> Participants { get; set; } = new List<TaskUser>();

        
    }
}
