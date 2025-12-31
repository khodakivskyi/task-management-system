using backend.Models;
using backend.Models.DTO;

namespace backend.Interfaces;

public interface ITaskSearchService
{
    Task<IEnumerable<TaskSearchResultDto>> SearchTasksAsync(TaskSearchFilter taskSearchFilter);
}
