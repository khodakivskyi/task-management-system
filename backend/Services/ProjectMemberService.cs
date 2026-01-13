using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services.Interfaces;

namespace backend.Services;

/// <summary>
/// Service for ProjectMember operations with business logic and validation
/// </summary>
public class ProjectMemberService : IProjectMemberService
{
    private readonly IProjectMemberRepository _projectMemberRepository;
    private readonly IRepository<Project> _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRepository<ProjectRole>? _projectRoleRepository;

    private const string ProjectEntity = nameof(Project);
    private const string UserEntity = nameof(User);
    private const string RoleEntity = "Role";

    public ProjectMemberService(
        IProjectMemberRepository projectMemberRepository,
        IRepository<Project> projectRepository,
        IUserRepository userRepository,
        IRepository<ProjectRole>? projectRoleRepository = null)
    {
        _projectMemberRepository = projectMemberRepository ?? throw new ArgumentNullException(nameof(projectMemberRepository));
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _projectRoleRepository = projectRoleRepository;
    }

    public async Task<ProjectMember> AddMemberAsync(int projectId, int userId, int roleId, int requestingUserId)
    {
        ValidationHelper.ValidateId(projectId, ProjectEntity);
        ValidationHelper.ValidateId(userId, UserEntity);
        ValidationHelper.ValidateId(roleId, RoleEntity);

        var project = await EntityValidationHelper.EnsureEntityExistsAsync(projectId, _projectRepository, ProjectEntity);

        AuthorizationHelper.EnsureOwnership(project.OwnerId, requestingUserId, "project members", "add");

        await ProjectMemberHelper.ValidateProjectMemberAsync(
            projectId,
            userId,
            roleId,
            _projectRepository,
            _userRepository,
            _projectRoleRepository);

        // Check if member already exists
        var existingMember = await _projectMemberRepository.GetByProjectAndUserAsync(projectId, userId);
        if (existingMember != null)
        {
            throw new ConflictException($"User {userId} is already a member of project {projectId}");
        }

        var projectMember = new ProjectMember
        {
            ProjectId = projectId,
            UserId = userId,
            RoleId = roleId,
            JoinedAt = DateTime.UtcNow
        };

        var id = await _projectMemberRepository.CreateAsync(projectMember);
        projectMember.Id = id;
        return projectMember;
    }

    public async Task RemoveMemberAsync(int projectId, int userId, int requestingUserId)
    {
        ValidationHelper.ValidateId(projectId, ProjectEntity);
        ValidationHelper.ValidateId(userId, UserEntity);

        var project = await EntityValidationHelper.EnsureEntityExistsAsync(projectId, _projectRepository, ProjectEntity);

        AuthorizationHelper.EnsureOwnership(project.OwnerId, requestingUserId, "project members", "remove");

        // Check if member exists
        var member = await _projectMemberRepository.GetByProjectAndUserAsync(projectId, userId);
        if (member == null)
        {
            throw new NotFoundException("User is not a member of this project");
        }

        var deleted = await _projectMemberRepository.DeleteAsync(member.Id);
        if (!deleted)
        {
            throw new NotFoundException("Failed to remove user from project");
        }
    }

    public async Task<ProjectMember> UpdateMemberRoleAsync(int projectId, int userId, int newRoleId, int requestingUserId)
    {
        ValidationHelper.ValidateId(projectId, ProjectEntity);
        ValidationHelper.ValidateId(userId, UserEntity);
        ValidationHelper.ValidateId(newRoleId, RoleEntity);

        var project = await EntityValidationHelper.EnsureEntityExistsAsync(projectId, _projectRepository, ProjectEntity);

        AuthorizationHelper.EnsureOwnership(project.OwnerId, requestingUserId, "project members", "update role for");

        // Check if member exists
        var member = await _projectMemberRepository.GetByProjectAndUserAsync(projectId, userId);
        if (member == null)
        {
            throw new NotFoundException("User is not a member of this project");
        }

        // Validate RoleId exists if repository is provided
        if (_projectRoleRepository != null)
        {
            var role = await _projectRoleRepository.GetByIdAsync(newRoleId);
            if (role == null)
            {
                throw new NotFoundException("Project role not found");
            }
        }

        member.RoleId = newRoleId;

        var updated = await _projectMemberRepository.UpdateAsync(member);
        if (!updated)
        {
            throw new NotFoundException("Failed to update user role in project");
        }

        return member;
    }

    public async Task<IEnumerable<ProjectMember>> GetProjectMembersAsync(int projectId)
    {
        ValidationHelper.ValidateId(projectId, ProjectEntity);

        // Validate ProjectId exists
        await EntityValidationHelper.EnsureEntityExistsAsync(projectId, _projectRepository, ProjectEntity);

        return await _projectMemberRepository.GetByProjectIdAsync(projectId);
    }
}
