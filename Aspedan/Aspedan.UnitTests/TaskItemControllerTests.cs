using Aspedan.Controllers;
using Aspedan.Entities.Dtos;
using Aspedan.Entities.Models;
using Aspedan.Entities.Validation;
using Aspedan.Persistence;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Aspedan.Tests
{
	public class TaskItemControllerTests
	{
		private readonly AspedanDbContext _dbContext;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<IValidator<TaskItemDto>> _validatorMock;
		private readonly TaskItemController _controller;

		public TaskItemControllerTests()
		{
			var options = new DbContextOptionsBuilder<AspedanDbContext>()
				.UseInMemoryDatabase(databaseName: "AspedanDb")
				.Options;
			_dbContext = new AspedanDbContext(options);
			_mapperMock = new Mock<IMapper>();
			_validatorMock = new Mock<IValidator<TaskItemDto>>();
			_controller = new TaskItemController(_dbContext, _mapperMock.Object, new TaskItemValidation());

			// Seed the in-memory database
			_dbContext.TaskItems.Add(new TaskItem { Id = 1, Title = "Test Task", Description = "Description", DueDate = DateTime.UtcNow, IsCompleted = false });
			_dbContext.SaveChanges();
		}

		[Fact]
		public async Task Get_ReturnsOkResult_WithListOfTaskItems()
		{
			// Act
			var result = await _controller.Get(CancellationToken.None);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnValue = Assert.IsType<List<TaskItem>>(okResult.Value);
			Assert.Single(returnValue);
		}

		[Fact]
		public async Task Create_ReturnsBadRequest_WhenValidationFails()
		{
			// Arrange
			var taskItemDto = new TaskItemDto { Title = "Invalid Task", Description = "Description" };
			var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Title", "Error") });
			_validatorMock.Setup(v => v.ValidateAsync(taskItemDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(validationResult);

			// Act
			var result = await _controller.Create(taskItemDto, CancellationToken.None);

			// Assert
			Assert.IsType<StatusCodeResult>(result);
		}

		[Fact]
		public async Task Create_ReturnsOkResult_WhenTaskItemIsCreated()
		{
			// Arrange
			var taskItemDto = new TaskItemDto { Title = "Valid Task", Description = "Description", DueDate = DateTime.UtcNow, IsCompleted = false };
			var validationResult = new ValidationResult();
			_validatorMock.Setup(v => v.ValidateAsync(taskItemDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(validationResult);
			_mapperMock.Setup(m => m.Map<TaskItem>(taskItemDto))
					   .Returns(new TaskItem { Title = taskItemDto.Title, Description = taskItemDto.Description, DueDate = taskItemDto.DueDate, IsCompleted = taskItemDto.IsCompleted });

			// Act
			var result = await _controller.Create(taskItemDto, CancellationToken.None);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Valid Task Created!", okResult.Value);
		}

		[Fact]
		public async Task Update_ReturnsBadRequest_WhenValidationFails()
		{
			// Arrange
			var taskItemDto = new TaskItemDto { Title = "Invalid Task", Description = "Description" };
			var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Title", "Error") });
			_validatorMock.Setup(v => v.ValidateAsync(taskItemDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(validationResult);

			// Act
			var result = await _controller.Update(1, taskItemDto, CancellationToken.None);

			// Assert
			Assert.IsType<StatusCodeResult>(result);
		}

		[Fact]
		public async Task Update_ReturnsOkResult_WhenTaskItemIsUpdated()
		{
			// Arrange
			var taskItemDto = new TaskItemDto { Title = "Valid Task", Description = "Description", DueDate = DateTime.UtcNow, IsCompleted = false };
			var validationResult = new ValidationResult();
			_validatorMock.Setup(v => v.ValidateAsync(taskItemDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(validationResult);
			_mapperMock.Setup(m => m.Map<TaskItem>(taskItemDto))
					   .Returns(new TaskItem { Title = taskItemDto.Title, Description = taskItemDto.Description, DueDate = taskItemDto.DueDate, IsCompleted = taskItemDto.IsCompleted });

			// Act
			var result = await _controller.Update(1, taskItemDto, CancellationToken.None);

			// Assert
			Assert.IsType<StatusCodeResult>(result);
		}

		[Fact]
		public async Task Delete_ReturnsNotFound_WhenTaskItemDoesNotExist()
		{
			// Act
			var result = await _controller.Delete(99, CancellationToken.None); // ID that doesn't exist

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal("Task does not exist!", notFoundResult.Value);
		}

		[Fact]
		public async Task Delete_ReturnsOkResult_WhenTaskItemIsDeleted()
		{
			// Arrange
			var taskItem = new TaskItem { Id = 2, Title = "Test Task 2", Description = "Description", DueDate = DateTime.UtcNow, IsCompleted = false };
			_dbContext.TaskItems.Add(taskItem);
			await _dbContext.SaveChangesAsync();

			// Act
			var result = await _controller.Delete(2, CancellationToken.None);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Test Task 2 Deleted!", okResult.Value);
		}
	}
}
