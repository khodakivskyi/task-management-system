using backend.Models;

namespace backend.Infrastructure.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<(bool loginExists, bool emailExists)> CheckUserExistsAsync(string login, string email);
    Task<User?> GetByLoginOrEmailAsync(string loginOrEmail);
}
