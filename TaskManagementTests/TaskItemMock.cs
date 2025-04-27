using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Enums;
using TaskManagement.Models;

namespace TaskManagementTests
{
    public class TaskItemMock
    {
        public TaskItem Create(Guid? taskId = null,
            string title = "Title Test",
            string description = "Description Test",
            Priority priority = Priority.Medium,
            Status status = Status.ToDo,
            DateTime? createdDate = null,
            DateTime? dueDate = null,
            Guid? reporterId = null)
        {
            return new TaskItem
            {
                TaskId = taskId ?? Guid.NewGuid(),
                Title = title,
                Description = description,
                Priority = priority,
                Status = status,
                CreatedDate = createdDate ?? DateTime.UtcNow,
                DueDate = dueDate,
                ReporterId = reporterId ?? Guid.NewGuid()
            };

        }


    }
}
