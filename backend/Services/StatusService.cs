using backend.Exceptions;
using backend.Interfaces;
using backend.Models;

namespace backend.Services;

/// <summary>
/// Service for Status read-only operations
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
        if (id <= 0)
        {
            throw new BadRequestException("Status id must be greater than 0");
        }

        return await _statusRepository.GetByIdAsync(id);
    }
}
