using backend.CodeFirst;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

namespace backend.CodeFirst;

/// <summary>
/// Example of how to configure Code-First DbContext in Program.cs
/// This is a reference implementation showing the correct setup
/// </summary>
public static class ProgramExample
{
    /// <summary>
    /// Example configuration for Code-First DbContext
    /// Shows how to register TaskManagementCodeFirstDbContext through dependency injection
    /// </summary>
    public static void ConfigureCodeFirstDbContext(WebApplicationBuilder builder)
    {
        // Load environment variables
        Env.Load();

        // Get connection string from environment
        string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
            ?? throw new InvalidOperationException("DB_CONNECTION_STRING is not set");

        // Register Code-First DbContext
        // Connection string is provided through DbContextOptions, NOT in OnConfiguring
        builder.Services.AddDbContext<TaskManagementCodeFirstDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Register Code-First demo
        builder.Services.AddScoped<CodeFirstDemo>();
    }

    /// <summary>
    /// Example of how to use Code-First DbContext in Program.cs
    /// </summary>
    public static async Task RunCodeFirstDemo(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var codeFirstDemo = scope.ServiceProvider.GetRequiredService<CodeFirstDemo>();
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

            await codeFirstDemo.RunDemonstrationAsync(cts.Token);
        }
    }
}

/*
 * INTEGRATION INSTRUCTIONS:
 * 
 * To integrate Code-First DbContext into your existing Program.cs:
 * 
 * 1. Add the following to your builder.Services configuration:
 * 
 *    builder.Services.AddDbContext<TaskManagementCodeFirstDbContext>(options =>
 *        options.UseNpgsql(connectionString));
 * 
 *    builder.Services.AddScoped<CodeFirstDemo>();
 * 
 * 2. To run the demonstration, add this in your app initialization:
 * 
 *    using (var scope = app.Services.CreateScope())
 *    {
 *        var codeFirstDemo = scope.ServiceProvider.GetRequiredService<CodeFirstDemo>();
 *        await codeFirstDemo.RunDemonstrationAsync();
 *    }
 * 
 * NOTE: The Code-First DbContext is separate from the Database-First DbContext
 * Both can coexist in the same application if needed
 */

