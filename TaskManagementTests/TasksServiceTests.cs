using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Data;
using TaskManagement.DTOs;
using TaskManagement.Enums;
using TaskManagement.Models;
using TaskManagement.Repositories;
using TaskManagement.Services;

namespace TaskManagementTests
{
    [TestClass]
    public class TasksServiceTests
    {
        private Mock<ITasksRepository> _mockRepository;
        private Mock<IMapper> _mockMapper;
        private TaskItemMock _taskMock;
        private TasksService _tasksService; 
        private List<TaskItem> _tasks;
        private Guid _userIdTest;


        [TestInitialize]
        public void Initialize()
        {
            
            _mockRepository = new Mock<ITasksRepository>();
            _mockMapper = new Mock<IMapper>();
            _tasksService = new TasksService(_mockRepository.Object, _mockMapper.Object);

            _taskMock = new TaskItemMock();
            // Initialize test data
            _tasks = new List<TaskItem>()
            {
                _taskMock.Create(reporterId: _userIdTest, title: "Task Test 1"),
                _taskMock.Create(reporterId: _userIdTest, title: "Task Test 2"),
                _taskMock.Create(reporterId: Guid.NewGuid(), title: "Task Test 3")
            };

        }

        [TestMethod]
        public async Task GetAllTasks_ShouldReturnTasks()
        {
            //Arrange
            var userTasks = _tasks.Where(t => t.ReporterId == _userIdTest).ToList();


            _mockRepository.Setup(repository => repository.GetTasksUserAsync(_userIdTest)).ReturnsAsync(userTasks);

            //Act
            var result = await _tasksService.GetTasksUserAsync(_userIdTest);

            // Assert
            result.Should().BeEquivalentTo(userTasks);
            _mockRepository.Verify(repository => repository.GetTasksUserAsync(_userIdTest), Times.Once());
        
        }

        [TestMethod]
        public async Task GetTaskById_ShouldReturnTask()
        {
            // Arrange
            TaskItem expectedTask = _tasks[0];

            _mockRepository.Setup(repository => repository.GetTaskByIdAsync(expectedTask.TaskId)).ReturnsAsync(expectedTask);

            //Act
            var result = await _tasksService.GetTaskByIdAsync(expectedTask.TaskId);

            //Assert
            result.Should().BeEquivalentTo(expectedTask);
            _mockRepository.Verify(repository => repository.GetTaskByIdAsync(expectedTask.TaskId), Times.Once());
        }

        [TestMethod]
        public async Task CreateTask_ShouldCreateTask()
        {
            // Arrange
            CreateUpdateTaskDTO taskDTO = new CreateUpdateTaskDTO
            {
                Title = "New Task",
                Description = "New Task Description",
                Priority = Priority.High,
                Status = Status.ToDo,
                DueDate = DateTime.UtcNow.AddDays(3),
            };

            var mappedTask = _taskMock.Create(
                title: taskDTO.Title,
                description: taskDTO.Description,
                priority: taskDTO.Priority,
                status: taskDTO.Status,
                dueDate: taskDTO.DueDate,
                reporterId: Guid.Empty
                );

            var createdTask = _taskMock.Create(
                title: taskDTO.Title,
                description: taskDTO.Description,
                priority: taskDTO.Priority,
                status: taskDTO.Status,
                reporterId: _userIdTest);

            _mockMapper.Setup(mapper => mapper.Map<TaskItem>(taskDTO)).Returns(mappedTask);

            _mockRepository.Setup(repository => repository.CreateTaskAsync(It.Is<TaskItem>(t =>
                t.Title == taskDTO.Title &&
                t.Description == taskDTO.Description &&
                t.Priority == taskDTO.Priority &&
                t.Status == taskDTO.Status &&
                t.ReporterId == _userIdTest)))
                .ReturnsAsync(createdTask);

            //Act
            var result = await _tasksService.CreateTaskAsync(taskDTO, _userIdTest);

            // Assert
            result.Should().BeEquivalentTo(createdTask);
            _mockMapper.Verify(mapper => mapper.Map<TaskItem>(taskDTO), Times.Once());
            _mockRepository.Verify(repository => repository.CreateTaskAsync(It.Is<TaskItem>(t =>
               t.Title == taskDTO.Title &&
               t.Description == taskDTO.Description &&
               t.Priority == taskDTO.Priority &&
               t.Status == taskDTO.Status &&
               t.ReporterId == _userIdTest)), Times.Once);

        }

        [TestMethod]
        public async Task DeleteTask_ShouldDelete()
        {
            // Arrange
            var existingTask = _tasks[0];

            _mockRepository.Setup(repository => repository.GetTaskByIdAsync(existingTask.TaskId)).ReturnsAsync(existingTask);
            _mockRepository.Setup(repository => repository.DeleteTaskAsync(existingTask.TaskId)).ReturnsAsync(true);

            // Act
            var result = await _tasksService.DeleteTaskAsync(existingTask.TaskId);

            //Assert
            result.Should().BeTrue();
            _mockRepository.Verify(repository => repository.GetTaskByIdAsync(existingTask.TaskId), Times.Once);
            _mockRepository.Verify(repository => repository.DeleteTaskAsync(existingTask.TaskId), Times.Once);
        }
    }
}
