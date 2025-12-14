using backend.EFModels;

namespace backend.EFRepositories;

/// <summary>
/// Repository interface for User entity using Entity Framework Core
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByLoginAsync(string login);
    Task<User?> GetWithDetailsAsync(int userId);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByLoginAsync(string login);
}

