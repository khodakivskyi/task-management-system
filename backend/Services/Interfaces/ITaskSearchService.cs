using backend.Models;
using backend.Models.DTO;
using backend.Models.Filters;

namespace backend.Services.Interfaces;

public interface ITaskSearchService
{
    Task<IEnumerable<TaskSearchResultDto>> SearchTasksAsync(TaskSearchFilter taskSearchFilter);
}
