using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using FluentAssertions;
using Moq;

namespace backend.Tests.Helpers;

public class TaskHelperTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRepository<Status>> _mockStatusRepository;
    private readonly Mock<IRepository<Category>> _mockCategoryRepository;
    private readonly Mock<IRepository<Project>> _mockProjectRepository;

    public TaskHelperTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockStatusRepository = new Mock<IRepository<Status>>();
        _mockCategoryRepository = new Mock<IRepository<Category>>();
        _mockProjectRepository = new Mock<IRepository<Project>>();
    }

    [Fact]
    public async Task ValidateTaskAsync_WithValidTask_DoesNotThrow()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "Test Task",
            OwnerId = 1,
            StatusId = 1,
            Priority = 3
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockStatusRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Status { Id = 1, Name = "To Do" });

        // Act
        var act = async () => await TaskHelper.ValidateTaskAsync(
            task,
            _mockUserRepository.Object,
            _mockStatusRepository.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateTaskAsync_WithEmptyTitle_ThrowsValidationException()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "",
            OwnerId = 1,
            StatusId = 1
        };

        // Act
        var act = async () => await TaskHelper.ValidateTaskAsync(
            task,
            _mockUserRepository.Object,
            _mockStatusRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Task title is required");
    }

    [Fact]
    public async Task ValidateTaskAsync_WithNullTitle_ThrowsValidationException()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = null!,
            OwnerId = 1,
            StatusId = 1
        };

        // Act
        var act = async () => await TaskHelper.ValidateTaskAsync(
            task,
            _mockUserRepository.Object,
            _mockStatusRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Task title is required");
    }

    [Fact]
    public async Task ValidateTaskAsync_WithWhitespaceTitle_ThrowsValidationException()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "   ",
            OwnerId = 1,
            StatusId = 1
        };

        // Act
        var act = async () => await TaskHelper.ValidateTaskAsync(
            task,
            _mockUserRepository.Object,
            _mockStatusRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Task title is required");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(10)]
    [InlineData(-1)]
    public async Task ValidateTaskAsync_WithInvalidPriority_ThrowsValidationException(int priority)
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "Test Task",
            OwnerId = 1,
            StatusId = 1,
            Priority = priority
        };

        // Act
        var act = async () => await TaskHelper.ValidateTaskAsync(
            task,
            _mockUserRepository.Object,
            _mockStatusRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Priority must be between 1 and 5");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task ValidateTaskAsync_WithValidPriority_DoesNotThrow(int priority)
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "Test Task",
            OwnerId = 1,
            StatusId = 1,
            Priority = priority
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockStatusRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Status { Id = 1, Name = "To Do" });

        // Act
        var act = async () => await TaskHelper.ValidateTaskAsync(
            task,
            _mockUserRepository.Object,
            _mockStatusRepository.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateTaskAsync_WithNonExistentOwnerId_ThrowsNotFoundException()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "Test Task",
            OwnerId = 999,
            StatusId = 1
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await TaskHelper.ValidateTaskAsync(
            task,
            _mockUserRepository.Object,
            _mockStatusRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task ValidateTaskAsync_WithNonExistentStatusId_ThrowsNotFoundException()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "Test Task",
            OwnerId = 1,
            StatusId = 999
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockStatusRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Status?)null);

        // Act
        var act = async () => await TaskHelper.ValidateTaskAsync(
            task,
            _mockUserRepository.Object,
            _mockStatusRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Status not found");
    }

    [Fact]
    public async Task ValidateTaskAsync_WithInvalidCategoryId_ThrowsNotFoundException()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "Test Task",
            OwnerId = 1,
            StatusId = 1,
            CategoryId = 999
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockStatusRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Status { Id = 1, Name = "To Do" });
        _mockCategoryRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Category?)null);

        // Act
        var act = async () => await TaskHelper.ValidateTaskAsync(
            task,
            _mockUserRepository.Object,
            _mockStatusRepository.Object,
            _mockCategoryRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Category not found");
    }

    [Fact]
    public async Task ValidateTaskAsync_WithInvalidProjectId_ThrowsNotFoundException()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "Test Task",
            OwnerId = 1,
            StatusId = 1,
            ProjectId = 999
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockStatusRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Status { Id = 1, Name = "To Do" });
        _mockProjectRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Project?)null);

        // Act
        var act = async () => await TaskHelper.ValidateTaskAsync(
            task,
            _mockUserRepository.Object,
            _mockStatusRepository.Object,
            _mockCategoryRepository.Object,
            _mockProjectRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Project not found");
    }

    [Fact]
    public async Task ValidateTaskAsync_WithValidOptionalFields_DoesNotThrow()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "Test Task",
            OwnerId = 1,
            StatusId = 1,
            CategoryId = 1,
            ProjectId = 1,
            Priority = 5
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockStatusRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Status { Id = 1, Name = "To Do" });
        _mockCategoryRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Category { Id = 1, Name = "Bug", Color = "#FF0000" });
        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Project { Id = 1, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });

        // Act
        var act = async () => await TaskHelper.ValidateTaskAsync(
            task,
            _mockUserRepository.Object,
            _mockStatusRepository.Object,
            _mockCategoryRepository.Object,
            _mockProjectRepository.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateTaskAsync_WithNullPriority_DoesNotThrow()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "Test Task",
            OwnerId = 1,
            StatusId = 1,
            Priority = null
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockStatusRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Status { Id = 1, Name = "To Do" });

        // Act
        var act = async () => await TaskHelper.ValidateTaskAsync(
            task,
            _mockUserRepository.Object,
            _mockStatusRepository.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
