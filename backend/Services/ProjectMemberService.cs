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
    private readonly IRepository<ProjectMember> _projectMemberRepository;
    private readonly IRepository<Project> _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRepository<ProjectRole>? _projectRoleRepository;

    public ProjectMemberService(
        IRepository<ProjectMember> projectMemberRepository,
        IRepository<Project> projectRepository,
        IUserRepository userRepository,
        IRepository<ProjectRole>? projectRoleRepository = null)
    {
        _projectMemberRepository = projectMemberRepository ?? throw new ArgumentNullException(nameof(projectMemberRepository));
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _projectRoleRepository = projectRoleRepository;
    }

    public async Task<ProjectMember> AddMemberAsync(int projectId, int userId, int roleId)
    {
        ValidationHelper.ValidateId(projectId, "Project");
        ValidationHelper.ValidateId(userId, "User");
        ValidationHelper.ValidateId(roleId, "Role");

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

    public async Task RemoveMemberAsync(int projectId, int userId)
    {
        ValidationHelper.ValidateId(projectId, "Project");
        ValidationHelper.ValidateId(userId, "User");

        // Check if member exists
        var member = await _projectMemberRepository.GetByProjectAndUserAsync(projectId, userId);
        if (member == null)
        {
            throw new NotFoundException($"User {userId} is not a member of project {projectId}");
        }

        var deleted = await _projectMemberRepository.DeleteAsync(member.Id);
        if (!deleted)
        {
            throw new NotFoundException($"Failed to remove user {userId} from project {projectId}");
        }
    }

    public async Task<ProjectMember> UpdateMemberRoleAsync(int projectId, int userId, int newRoleId)
    {
        ValidationHelper.ValidateId(projectId, "Project");
        ValidationHelper.ValidateId(userId, "User");
        ValidationHelper.ValidateId(newRoleId, "Role");

        // Check if member exists
        var member = await _projectMemberRepository.GetByProjectAndUserAsync(projectId, userId);
        if (member == null)
        {
            throw new NotFoundException($"User {userId} is not a member of project {projectId}");
        }

        // Validate RoleId exists if repository is provided
        if (_projectRoleRepository != null)
        {
            var role = await _projectRoleRepository.GetByIdAsync(newRoleId);
            if (role == null)
            {
                throw new NotFoundException($"Project role with id {newRoleId} not found");
            }
        }

        member.RoleId = newRoleId;

        var updated = await _projectMemberRepository.UpdateAsync(member);
        if (!updated)
        {
            throw new NotFoundException($"Failed to update role for user {userId} in project {projectId}");
        }

        return member;
    }

    public async Task<IEnumerable<ProjectMember>> GetProjectMembersAsync(int projectId)
    {
        ValidationHelper.ValidateId(projectId, "Project");

        // Validate ProjectId exists
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            throw new NotFoundException($"Project with id {projectId} not found");
        }

        return await _projectMemberRepository.GetByProjectIdAsync(projectId);
    }
}
