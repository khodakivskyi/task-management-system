using backend.Models;
using backend.Models.DTO;

namespace backend.Services.Interfaces;

public interface ITaskSearchService
{
    Task<IEnumerable<TaskSearchResultDto>> SearchTasksAsync(TaskSearchFilter taskSearchFilter);
}
