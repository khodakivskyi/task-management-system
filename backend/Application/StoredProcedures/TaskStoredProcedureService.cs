using System.Data;
using Dapper;
using Npgsql;
using NpgsqlTypes;

namespace backend.Application.StoredProcedures;

/// <summary>
/// Service for calling stored procedures related to Tasks
/// </summary>
public class TaskStoredProcedureService
{
    private readonly string _connectionString;

    public TaskStoredProcedureService(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Calls CreateTaskWithValidation stored procedure
    /// Demonstrates: INPUT parameters, OUTPUT parameters, and return code handling
    /// </summary>
    public async Task<CreateTaskResult> CreateTaskWithValidationAsync(CreateTaskInput input)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        // Configure command for stored procedure
        await using var command = new NpgsqlCommand("create_task_with_validation", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        // INPUT parameters
        command.Parameters.AddWithValue("p_owner_id", input.OwnerId);
        command.Parameters.AddWithValue("p_status_id", input.StatusId);
        command.Parameters.AddWithValue("p_category_id", (object?)input.CategoryId ?? DBNull.Value);
        command.Parameters.AddWithValue("p_project_id", (object?)input.ProjectId ?? DBNull.Value);
        command.Parameters.AddWithValue("p_title", input.Title);
        command.Parameters.AddWithValue("p_description", (object?)input.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("p_priority", (object?)input.Priority ?? DBNull.Value);
        command.Parameters.AddWithValue("p_deadline", (object?)input.Deadline ?? DBNull.Value);
        command.Parameters.AddWithValue("p_estimated_hours", input.EstimatedHours);
        command.Parameters.AddWithValue("p_actual_hours", input.ActualHours);

        // OUTPUT parameters
        var taskIdParam = new NpgsqlParameter("p_task_id", NpgsqlDbType.Integer)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(taskIdParam);

        var createdAtParam = new NpgsqlParameter("p_created_at", NpgsqlDbType.Timestamp)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(createdAtParam);

        var messageParam = new NpgsqlParameter("p_message", NpgsqlDbType.Varchar)
        {
            Direction = ParameterDirection.Output,
            Size = 255
        };
        command.Parameters.Add(messageParam);

        try
        {
            // Execute stored procedure
            await command.ExecuteNonQueryAsync();

            // Retrieve OUTPUT parameters
            var taskId = taskIdParam.Value != DBNull.Value ? (int)taskIdParam.Value! : 0;
            var createdAt = createdAtParam.Value != DBNull.Value ? (DateTime)createdAtParam.Value! : DateTime.MinValue;
            var message = messageParam.Value?.ToString() ?? string.Empty;

            // Check return code through message (success/error indicator)
            if (string.IsNullOrEmpty(message) || message.Contains("Error") || taskId == 0)
            {
                return new CreateTaskResult
                {
                    Success = false,
                    TaskId = 0,
                    CreatedAt = DateTime.MinValue,
                    Message = message
                };
            }

            return new CreateTaskResult
            {
                Success = true,
                TaskId = taskId,
                CreatedAt = createdAt,
                Message = message
            };
        }
        catch (PostgresException ex)
        {
            return new CreateTaskResult
            {
                Success = false,
                TaskId = 0,
                CreatedAt = DateTime.MinValue,
                Message = $"Database error: {ex.Message}"
            };
        }
    }
}

