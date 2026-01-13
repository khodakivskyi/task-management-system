using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using FluentAssertions;
using Moq;

namespace backend.Tests.Helpers;

public class FavoriteHelperTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IEntityTypeRepository> _mockEntityTypeRepository;
    private readonly Mock<IRepository<TaskModel>> _mockTaskRepository;
    private readonly Mock<IRepository<Project>> _mockProjectRepository;

    public FavoriteHelperTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockEntityTypeRepository = new Mock<IEntityTypeRepository>();
        _mockTaskRepository = new Mock<IRepository<TaskModel>>();
        _mockProjectRepository = new Mock<IRepository<Project>>();
    }

    #region ValidateFavoriteAsync Tests

    [Fact]
    public async Task ValidateFavoriteAsync_WithValidData_DoesNotThrow()
    {
        // Arrange
        var userId = 1;
        var entityTypeId = 1;

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(new User { Id = userId, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(entityTypeId))
            .ReturnsAsync(new EntityType { Id = entityTypeId, Name = "task" });

        // Act
        var act = async () => await FavoriteHelper.ValidateFavoriteAsync(
            userId, entityTypeId, _mockUserRepository.Object, _mockEntityTypeRepository.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateFavoriteAsync_WithNonExistentUser_ThrowsNotFoundException()
    {
        // Arrange
        var userId = 999;
        var entityTypeId = 1;

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await FavoriteHelper.ValidateFavoriteAsync(
            userId, entityTypeId, _mockUserRepository.Object, _mockEntityTypeRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task ValidateFavoriteAsync_WithNonExistentEntityType_ThrowsNotFoundException()
    {
        // Arrange
        var userId = 1;
        var entityTypeId = 999;

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(new User { Id = userId, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(entityTypeId))
            .ReturnsAsync((EntityType?)null);

        // Act
        var act = async () => await FavoriteHelper.ValidateFavoriteAsync(
            userId, entityTypeId, _mockUserRepository.Object, _mockEntityTypeRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Entity type not found");
    }

    #endregion

    #region ValidateEntityOwnershipAsync Tests

    [Fact]
    public async Task ValidateEntityOwnershipAsync_WithValidTaskOwnership_DoesNotThrow()
    {
        // Arrange
        var userId = 1;
        var entityTypeId = 1;
        var entityId = 10;

        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(entityTypeId))
            .ReturnsAsync(new EntityType { Id = entityTypeId, Name = "task" });
        _mockTaskRepository.Setup(x => x.GetByIdAsync(entityId))
            .ReturnsAsync(new TaskModel { Id = entityId, OwnerId = userId, Title = "Test Task", StatusId = 1 });

        // Act
        var act = async () => await FavoriteHelper.ValidateEntityOwnershipAsync(
            userId, entityTypeId, entityId,
            _mockEntityTypeRepository.Object, _mockTaskRepository.Object, _mockProjectRepository.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateEntityOwnershipAsync_WithValidProjectOwnership_DoesNotThrow()
    {
        // Arrange
        var userId = 1;
        var entityTypeId = 2;
        var entityId = 20;

        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(entityTypeId))
            .ReturnsAsync(new EntityType { Id = entityTypeId, Name = "project" });
        _mockProjectRepository.Setup(x => x.GetByIdAsync(entityId))
            .ReturnsAsync(new Project
            {
                Id = entityId,
                OwnerId = userId,
                Name = "Test Project",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            });

        // Act
        var act = async () => await FavoriteHelper.ValidateEntityOwnershipAsync(
            userId, entityTypeId, entityId,
            _mockEntityTypeRepository.Object, _mockTaskRepository.Object, _mockProjectRepository.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateEntityOwnershipAsync_WithNonExistentEntityType_ThrowsNotFoundException()
    {
        // Arrange
        var userId = 1;
        var entityTypeId = 999;
        var entityId = 10;

        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(entityTypeId))
            .ReturnsAsync((EntityType?)null);

        // Act
        var act = async () => await FavoriteHelper.ValidateEntityOwnershipAsync(
            userId, entityTypeId, entityId,
            _mockEntityTypeRepository.Object, _mockTaskRepository.Object, _mockProjectRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Entity type not found");
    }

    [Fact]
    public async Task ValidateEntityOwnershipAsync_WithNonExistentTask_ThrowsNotFoundException()
    {
        // Arrange
        var userId = 1;
        var entityTypeId = 1;
        var entityId = 999;

        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(entityTypeId))
            .ReturnsAsync(new EntityType { Id = entityTypeId, Name = "task" });
        _mockTaskRepository.Setup(x => x.GetByIdAsync(entityId))
            .ReturnsAsync((TaskModel?)null);

        // Act
        var act = async () => await FavoriteHelper.ValidateEntityOwnershipAsync(
            userId, entityTypeId, entityId,
            _mockEntityTypeRepository.Object, _mockTaskRepository.Object, _mockProjectRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Task not found");
    }

    [Fact]
    public async Task ValidateEntityOwnershipAsync_WithNonExistentProject_ThrowsNotFoundException()
    {
        // Arrange
        var userId = 1;
        var entityTypeId = 2;
        var entityId = 999;

        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(entityTypeId))
            .ReturnsAsync(new EntityType { Id = entityTypeId, Name = "project" });
        _mockProjectRepository.Setup(x => x.GetByIdAsync(entityId))
            .ReturnsAsync((Project?)null);

        // Act
        var act = async () => await FavoriteHelper.ValidateEntityOwnershipAsync(
            userId, entityTypeId, entityId,
            _mockEntityTypeRepository.Object, _mockTaskRepository.Object, _mockProjectRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Project not found");
    }

    [Fact]
    public async Task ValidateEntityOwnershipAsync_WithDifferentTaskOwner_ThrowsUnauthorizedException()
    {
        // Arrange
        var userId = 1;
        var differentOwnerId = 2;
        var entityTypeId = 1;
        var entityId = 10;

        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(entityTypeId))
            .ReturnsAsync(new EntityType { Id = entityTypeId, Name = "task" });
        _mockTaskRepository.Setup(x => x.GetByIdAsync(entityId))
            .ReturnsAsync(new TaskModel
            {
                Id = entityId,
                OwnerId = differentOwnerId,
                Title = "Test Task",
                StatusId = 1
            });

        // Act
        var act = async () => await FavoriteHelper.ValidateEntityOwnershipAsync(
            userId, entityTypeId, entityId,
            _mockEntityTypeRepository.Object, _mockTaskRepository.Object, _mockProjectRepository.Object);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("You can only add your own tasks to favorites");
    }

    [Fact]
    public async Task ValidateEntityOwnershipAsync_WithDifferentProjectOwner_ThrowsUnauthorizedException()
    {
        // Arrange
        var userId = 1;
        var differentOwnerId = 2;
        var entityTypeId = 2;
        var entityId = 20;

        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(entityTypeId))
            .ReturnsAsync(new EntityType { Id = entityTypeId, Name = "project" });
        _mockProjectRepository.Setup(x => x.GetByIdAsync(entityId))
            .ReturnsAsync(new Project
            {
                Id = entityId,
                OwnerId = differentOwnerId,
                Name = "Test Project",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            });

        // Act
        var act = async () => await FavoriteHelper.ValidateEntityOwnershipAsync(
            userId, entityTypeId, entityId,
            _mockEntityTypeRepository.Object, _mockTaskRepository.Object, _mockProjectRepository.Object);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("You can only add your own projects to favorites");
    }

    [Fact]
    public async Task ValidateEntityOwnershipAsync_WithUnsupportedEntityType_ThrowsBadRequestException()
    {
        // Arrange
        var userId = 1;
        var entityTypeId = 3;
        var entityId = 30;

        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(entityTypeId))
            .ReturnsAsync(new EntityType { Id = entityTypeId, Name = "unsupported" });

        // Act
        var act = async () => await FavoriteHelper.ValidateEntityOwnershipAsync(
            userId, entityTypeId, entityId,
            _mockEntityTypeRepository.Object, _mockTaskRepository.Object, _mockProjectRepository.Object);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Unsupported entity type: unsupported");
    }

    [Theory]
    [InlineData("TASK")]
    [InlineData("Task")]
    [InlineData("PROJECT")]
    [InlineData("Project")]
    public async Task ValidateEntityOwnershipAsync_WithDifferentCasing_WorksCorrectly(string entityTypeName)
    {
        // Arrange
        var userId = 1;
        var entityTypeId = 1;
        var entityId = 10;
        var isTask = entityTypeName.ToLower() == "task";

        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(entityTypeId))
            .ReturnsAsync(new EntityType { Id = entityTypeId, Name = entityTypeName });

        if (isTask)
        {
            _mockTaskRepository.Setup(x => x.GetByIdAsync(entityId))
                .ReturnsAsync(new TaskModel { Id = entityId, OwnerId = userId, Title = "Test", StatusId = 1 });
        }
        else
        {
            _mockProjectRepository.Setup(x => x.GetByIdAsync(entityId))
                .ReturnsAsync(new Project
                {
                    Id = entityId,
                    OwnerId = userId,
                    Name = "Test",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30)
                });
        }

        // Act
        var act = async () => await FavoriteHelper.ValidateEntityOwnershipAsync(
            userId, entityTypeId, entityId,
            _mockEntityTypeRepository.Object, _mockTaskRepository.Object, _mockProjectRepository.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion
}
