using backend.Helpers;
using backend.Interfaces;
using backend.Models;

namespace backend.Services;

/// <summary>
/// Service for Status read-only operations
/// Statuses are default values in the database and cannot be created, updated, or deleted
/// </summary>
public class StatusService : IStatusService
{
    private readonly IRepository<Status> _statusRepository;

    public StatusService(IRepository<Status> statusRepository)
    {
        _statusRepository = statusRepository ?? throw new ArgumentNullException(nameof(statusRepository));
    }

    public async Task<IEnumerable<Status>> GetAllAsync()
    {
        return await _statusRepository.GetAllAsync();
    }

    public async Task<Status?> GetByIdAsync(int id)
    {
        ValidationHelper.ValidateId(id, "Status");
        return await _statusRepository.GetByIdAsync(id);
    }
}
