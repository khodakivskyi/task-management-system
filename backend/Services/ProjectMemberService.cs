using backend.Exceptions;
using backend.Interfaces;
using backend.Models;
using backend.Repositories;

namespace backend.Services;

/// <summary>
/// Service for ProjectMember operations with business logic and validation
/// </summary>
public class ProjectMemberService : IProjectMemberService
{
    private readonly ProjectMemberRepository _projectMemberRepository;
    private readonly IRepository<Project> _projectRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<ProjectRole>? _projectRoleRepository;

    public ProjectMemberService(
        ProjectMemberRepository projectMemberRepository,
        IRepository<Project> projectRepository,
        IRepository<User> userRepository,
        IRepository<ProjectRole>? projectRoleRepository = null)
    {
        _projectMemberRepository = projectMemberRepository ?? throw new ArgumentNullException(nameof(projectMemberRepository));
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _projectRoleRepository = projectRoleRepository;
    }

    public async Task<ProjectMember> AddMemberAsync(int projectId, int userId, int roleId)
    {
        if (projectId <= 0)
        {
            throw new BadRequestException("Project id must be greater than 0");
        }

        if (userId <= 0)
        {
            throw new BadRequestException("User id must be greater than 0");
        }

        if (roleId <= 0)
        {
            throw new BadRequestException("Role id must be greater than 0");
        }

        // Validate ProjectId exists
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            throw new NotFoundException($"Project with id {projectId} not found");
        }

        // Validate UserId exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"User with id {userId} not found");
        }

        // Validate RoleId exists if repository is provided
        if (_projectRoleRepository != null)
        {
            var role = await _projectRoleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new NotFoundException($"Project role with id {roleId} not found");
            }
        }

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
        if (projectId <= 0)
        {
            throw new BadRequestException("Project id must be greater than 0");
        }

        if (userId <= 0)
        {
            throw new BadRequestException("User id must be greater than 0");
        }

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
        if (projectId <= 0)
        {
            throw new BadRequestException("Project id must be greater than 0");
        }

        if (userId <= 0)
        {
            throw new BadRequestException("User id must be greater than 0");
        }

        if (newRoleId <= 0)
        {
            throw new BadRequestException("Role id must be greater than 0");
        }

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
        if (projectId <= 0)
        {
            throw new BadRequestException("Project id must be greater than 0");
        }

        // Validate ProjectId exists
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            throw new NotFoundException($"Project with id {projectId} not found");
        }

        return await _projectMemberRepository.GetByProjectIdAsync(projectId);
    }
}
