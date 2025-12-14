using backend.EFModels;
using Microsoft.EntityFrameworkCore;

namespace backend.EFRepositories;

/// <summary>
/// Repository implementation for User entity using Entity Framework Core
/// Demonstrates proper change tracking, AsNoTracking, and async operations with CancellationToken
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly TaskManagementDbContext _context;

    public UserRepository(TaskManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets a user by ID using FindAsync() for tracked entity
    /// </summary>
    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<User>()
            .FindAsync(new object[] { id }, cancellationToken);
    }

    /// <summary>
    /// Gets all users with optional change tracking
    /// Uses AsNoTracking() when trackChanges = false for better performance in read-only scenarios
    /// </summary>
    public async Task<IEnumerable<User>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<User>().AsQueryable();

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .OrderBy(u => u.Name)
            .ThenBy(u => u.Surname)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Creates a new user using context.Set<T>().Add() and SaveChangesAsync()
    /// </summary>
    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        await _context.Set<User>().AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    /// <summary>
    /// Updates an existing user using change tracking
    /// Supports both tracked and detached entities:
    /// - Tracked: Entity is already being tracked, changes are detected automatically
    /// - Detached: Entity is not tracked, Update() attaches and marks as modified
    /// </summary>
    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        // Check if entity is already tracked
        var existing = await _context.Set<User>()
            .FindAsync(new object[] { user.Id }, cancellationToken);

        if (existing == null)
            throw new InvalidOperationException($"User with Id {user.Id} not found");

        // Method 1: Tracked entity (automatic change detection)
        // EF Core automatically tracks changes to existing entity
        existing.Name = user.Name;
        existing.Surname = user.Surname;
        existing.Login = user.Login;
        existing.PasswordHash = user.PasswordHash;
        existing.Salt = user.Salt;

        // EF Core detects changes automatically for tracked entities
        await _context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    /// <summary>
    /// Deletes a user by ID using Remove() and SaveChangesAsync()
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Set<User>()
            .FindAsync(new object[] { id }, cancellationToken);

        if (user == null)
        {
            return false;
        }

        _context.Set<User>().Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

