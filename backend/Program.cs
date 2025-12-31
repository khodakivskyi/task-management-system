using backend.GraphQL;
using backend.Infrastructure.Migrations;
using backend.Interfaces;
using backend.Models;
using backend.Repositories;
using backend.Services;
using DotNetEnv;
using GraphQL.Execution;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        if (File.Exists(".env"))
        {
            Env.Load(". env");
        }
        else if (File.Exists(".. /.env"))
        {
            Env.Load("../.env");
        }

        var builder = WebApplication.CreateBuilder(args);
        var env = builder.Environment;

        // Getting .env variables for db
        string dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        string dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        string dbUser = Environment.GetEnvironmentVariable("DB_USER")
            ?? throw new InvalidOperationException("DB_USER is not set");
        string dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD")
            ?? throw new InvalidOperationException("DB_PASSWORD is not set");
        string dbName = Environment.GetEnvironmentVariable("DB_NAME")
            ?? throw new InvalidOperationException("DB_NAME is not set");

        // Compile connection string
        string connectionString = $"Host={dbHost};Port={dbPort};Username={dbUser};Password={dbPassword};Database={dbName}";

        builder.Services.AddSingleton(new MigrationRunner(connectionString, "Migrations", dbName));

        builder.Services.AddSingleton(connectionString);

        // Register Repositories
        builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        builder.Services.AddScoped<IRepository<Project>, ProjectRepository>();
        builder.Services.AddScoped<IRepository<Category>, CategoryRepository>();
        builder.Services.AddScoped<IRepository<Comment>, CommentRepository>();
        builder.Services.AddScoped<FavoriteRepository>();
        builder.Services.AddScoped<EntityTypeRepository>();
        builder.Services.AddScoped<IRepository<Status>, StatusRepository>();
        builder.Services.AddScoped<TaskHistoryRepository>();
        builder.Services.AddScoped<ProjectMemberRepository>();
        builder.Services.AddScoped<IRepository<User>, UserRepository>();
        builder.Services.AddScoped<IRepository<ProjectRole>, ProjectRoleRepository>();

        // Register Services
        builder.Services.AddScoped<ITaskService, TaskService>();
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IFavoriteService, FavoriteService>();
        builder.Services.AddScoped<IEntityTypeService, EntityTypeService>();
        builder.Services.AddScoped<IStatusService, StatusService>();
        builder.Services.AddScoped<ITaskHistoryService, TaskHistoryService>();
        builder.Services.AddScoped<IProjectMemberService, ProjectMemberService>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Exception handling
        builder.Services.AddScoped<IErrorInfoProvider, CustomErrorInfoProvider>();

        // Logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        //builder.Services.AddAuthorization();
        //builder.Services.AddAuthentication();

        var app = builder.Build();

        // Run migrations with retry logic
        using (var scope = app.Services.CreateScope())
        {
            var runner = scope.ServiceProvider.GetRequiredService<MigrationRunner>();

            var maxRetries = 10;
            var delay = TimeSpan.FromSeconds(5);

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    Console.WriteLine($"Attempting to run migrations (attempt {i + 1}/{maxRetries})...");
                    await runner.RunMigrationsAsync();
                    Console.WriteLine("SUCCESS: Migrations completed successfully!");
                    break;
                }
                catch (Exception ex)
                {
                    if (i == maxRetries - 1)
                    {
                        Console.WriteLine($"FAIL: Failed to run migrations after {maxRetries} attempts: {ex.Message}");
                        throw;
                    }

                    Console.WriteLine($"Database not ready, retrying in {delay.TotalSeconds}s...  ({i + 1}/{maxRetries})");
                    Console.WriteLine($"ERROR: {ex.Message}");
                    await Task.Delay(delay);
                }
            }
        }

        // Swagger UI
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger. json", "Task Management API V1");
                c.RoutePrefix = "swagger";
            });
        }

        //app.UseHttpsRedirection();
        //app.UseAuthentication();
        //app.UseAuthorization();

        Console.WriteLine("App started!");
        Console.WriteLine($"Swagger UI: http://localhost:{Environment.GetEnvironmentVariable("BACKEND_PORT") ?? "5000"}/swagger");

        app.Run();
    }
}
