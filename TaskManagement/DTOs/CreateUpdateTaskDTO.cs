using System.ComponentModel.DataAnnotations;
using TaskManagement.Enums;

namespace TaskManagement.DTOs
{
    public class CreateUpdateTaskDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Priority Priority { get; set; }
        public Status Status { get; set; }

        public DateTime DueDate { get; set; }

    }
}
