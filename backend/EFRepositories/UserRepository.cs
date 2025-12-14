using backend.EFModels;
using Microsoft.EntityFrameworkCore;

namespace backend.EFRepositories;

/// <summary>
/// Repository implementation for User entity using Entity Framework Core
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly TaskManagementDbContext _context;

    public UserRepository(TaskManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task<User?> GetWithDetailsAsync(int userId)
    {
        return await _context.GetUserWithDetailsAsync(userId);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .OrderBy(u => u.Name)
            .ThenBy(u => u.Surname)
            .ToListAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByLoginAsync(string login)
    {
        return await _context.UserExistsByLoginAsync(login);
    }
}

