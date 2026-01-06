namespace backend.Infrastructure.Migrations;

public static class MigrationExecutor
{
    public static async Task RunMigrationsWithRetryAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var runner = scope.ServiceProvider.GetRequiredService<MigrationRunner>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILogger<MigrationStartup>>();

        const int maxRetries = 10;
        var delay = TimeSpan.FromSeconds(5);

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                logger.LogInformation(
                    "Attempting to run migrations (attempt {Attempt}/{MaxRetries})...",
                    attempt, maxRetries);

                await runner.RunMigrationsAsync();

                logger.LogInformation("SUCCESS: Migrations completed successfully!");
                return;
            }
            catch (Exception ex)
            {
                if (attempt == maxRetries)
                {
                    logger.LogError(
                        ex,
                        "FAIL: Failed to run migrations after {MaxRetries} attempts",
                        maxRetries);
                    throw;
                }

                logger.LogWarning(
                    ex,
                    "Database not ready, retrying in {DelaySeconds}s... ({Attempt}/{MaxRetries})",
                    delay.TotalSeconds, attempt, maxRetries);

                await Task.Delay(delay);
            }
        }
    }
}
