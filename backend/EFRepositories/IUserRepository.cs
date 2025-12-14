using backend.EFModels;

namespace backend.EFRepositories;

/// <summary>
/// Repository interface for User entity using Entity Framework Core
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

