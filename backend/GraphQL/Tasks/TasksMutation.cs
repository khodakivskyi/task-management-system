using backend.GraphQL.Tasks.Inputs;
using backend.Models;
using backend.Services.Interfaces;

namespace backend.GraphQL.Mutations;

/// <summary>
/// GraphQL Mutation operations for Tasks
/// </summary>
public class TasksMutation
{
    public async Task<TaskModel> CreateTask(
        CreateTaskInput input,
        [Service] ITaskService taskService)
    {
        var task = new TaskModel
        {
            OwnerId = input.OwnerId,
            StatusId = input.StatusId,
            CategoryId = input.CategoryId,
            ProjectId = input.ProjectId,
            Title = input.Title,
            Description = input.Description,
            Priority = input.Priority,
            Deadline = input.Deadline,
            EstimatedHours = input.EstimatedHours,
            ActualHours = input.ActualHours
        };

        return await taskService.CreateAsync(task);
    }

    public async Task<TaskModel> UpdateTask(
        UpdateTaskInput input,
        [Service] ITaskService taskService)
    {
        var task = new TaskModel
        {
            Id = input.Id,
            StatusId = input.StatusId,
            CategoryId = input.CategoryId,
            ProjectId = input.ProjectId,
            OwnerId = input.OwnerId,
            Title = input.Title,
            Description = input.Description,
            Priority = input.Priority,
            Deadline = input.Deadline,
            EstimatedHours = input.EstimatedHours,
            ActualHours = input.ActualHours
        };

        return await taskService.UpdateAsync(task);
    }

    public async Task<bool> DeleteTask(
        DeleteTaskInput input,
        [Service] ITaskService taskService)
    {
        await taskService.DeleteAsync(input.Id, input.OwnerId);
        return true;
    }
}
