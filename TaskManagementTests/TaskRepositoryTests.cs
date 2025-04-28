using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Data;
using TaskManagement.Enums;
using TaskManagement.Models;
using TaskManagement.Repositories;

namespace TaskManagementTests
{
    [TestClass]
    public class TaskRepositoryTests
    {
        private ApplicationDbContext _context;
        private TasksRepository _tasksRepository;
        private TaskItemMock _taskMock;
        private List<TaskItem> _tasks;
        private Guid _userIdTest = Guid.NewGuid();
        

        [TestInitialize]
        public void Initialize()
        {
            // Set up in memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                                    .Options;

            _context = new ApplicationDbContext(options);

            _taskMock = new TaskItemMock();

            _tasksRepository = new TasksRepository(_context);

            // Initialize test data
            _tasks = new List<TaskItem>()
            {
                _taskMock.Create(reporterId: _userIdTest, title: "Task Test 1"),
                _taskMock.Create(reporterId: _userIdTest, title: "Task Test 2"),
                _taskMock.Create(reporterId: Guid.NewGuid(), title: "Task Test 3")
            };

            _context.AddRange(_tasks);
            _context.SaveChanges();

        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetTaskByIdAsync_TaskExists()
        {
            // Arrange
            TaskItem expectedTask = _tasks[0];

            // Act
            var result = await _tasksRepository.GetTaskByIdAsync(expectedTask.TaskId);
            
            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTask);

        }

        [TestMethod]
        public async Task GetTaskByIdAsync_WhenTaskDoesNotExists()
        {
            // Arrange 
            Guid taskId = Guid.NewGuid();

            // Act
            var result = await _tasksRepository.GetTaskByIdAsync(taskId);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetTasksUserAsync_TasksExist()
        {
            // Arrange
            var expectedTasks = _tasks.Where(t => t.ReporterId == _userIdTest).ToList();

            //Act
            var result = await _tasksRepository.GetTasksUserAsync(_userIdTest);

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedTasks);
        }

        [TestMethod]
        public async Task CreateTask_ShouldCreate()
        {
            // Arrange
            var newTask = _taskMock.Create(
                taskId: Guid.Empty,
                reporterId: _userIdTest,
                title: "New title test ",
                description: "New description test",
                status: Status.InProgress,
                priority: Priority.High);

            // Act
            var result = await _tasksRepository.CreateTaskAsync(newTask);

            //Assert
            result.Title.Should().Be(newTask.Title);
            result.Description.Should().Be(newTask.Description);
            result.Status.Should().Be(newTask.Status);
            result.Priority.Should().Be(newTask.Priority);
            result.ReporterId.Should().Be(newTask.ReporterId);

            // Check if task was successfully added to the db
            var taskDb = await _context.TaskItems.FindAsync(result.TaskId);
            taskDb.Should().NotBeNull();
            taskDb.Should().BeEquivalentTo(result);

        }

        [TestMethod]
        public async Task UpdateTask_ShouldUpdate()
        {
            // Arrange
            var existingTask = _tasks[0];

            var updatedTask = new TaskItem
            {
                Title = "Update Title",
                Description = "Update Description",
                Status = Status.Done,
                Priority = Priority.High,
                DueDate = DateTime.UtcNow.AddDays(2),

            };

            // Act
            var result = await _tasksRepository.UpdateTaskAsync(existingTask.TaskId, updatedTask);

            //Assert
            result.Should().NotBeNull();
            result.Title.Should().Be(updatedTask.Title);
            result.Description.Should().Be(updatedTask.Description);
            result.Status.Should().Be(updatedTask.Status);
            result.Priority.Should().Be(updatedTask.Priority);
            result.DueDate.Should().Be(updatedTask.DueDate);

            // TaskId, ReporterId and CreatedDate should not be updated
            result.TaskId.Should().Be(existingTask.TaskId);
            result.ReporterId.Should().Be(existingTask.ReporterId);
            result.CreatedDate.Should().Be(existingTask.CreatedDate);

            // Check if the task was updated in the db
            var taskDb = await _context.TaskItems.FindAsync(existingTask.TaskId);
            taskDb.Should().NotBeNull();
            taskDb.Should().BeEquivalentTo(result);
        }

        [TestMethod]
        public async Task DeleteTask_ShouldDelete()
        {
            // Arrange 
            var existingTaskId = _tasks[0].TaskId;

            // Act
            var result = await _tasksRepository.DeleteTaskAsync(existingTaskId);

            // Assert
            result.Should().BeTrue();

            // Check if the task was deleted from the db
            var taskDb = await _context.TaskItems.FindAsync(existingTaskId);
            taskDb.Should().BeNull();
        }

    }
}
