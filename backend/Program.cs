using backend.GraphQL;
using backend.GraphQL.Extensions;
using backend.Infrastructure.Migrations;
using backend.Infrastructure.Repositories;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services;
using backend.Services.Interfaces;
using DotNetEnv;

namespace backend;

public static partial class Program
{
    private static async Task Main(string[] args)
    {
        LoadEnv();

        var builder = WebApplication.CreateBuilder(args);

        // Build database connection string
        string connectionString = BuildConnectionString();
        builder.Services.AddSingleton(new MigrationRunner(connectionString, "Migrations/Scripts", GetEnv("DB_NAME")!));
        builder.Services.AddSingleton(connectionString);

        // Register repositories
        ConfigureRepositories(builder);

        // Register services
        ConfigureServices(builder);

        // GraphQL
        builder.Services.AddGraphQLServer()
            .AddQueryType<RootQuery>()
            .AddMutationType<RootMutation>()
            .AddErrorFilter<GraphQLErrorFilter>();

        // Logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        var app = builder.Build();

        // Run database migrations with retry logic
        await app.RunMigrationsWithRetryAsync();

        Console.WriteLine("App started!");
        Console.WriteLine($"GraphQL UI: http://localhost:{GetEnv("BACKEND_PORT") ?? "5000"}/graphql");

        app.MapGraphQL();
        await app.RunAsync();
    }

    #region Helpers

    // Load environment variables from .env file if it exists
    private static void LoadEnv()
    {
        if (File.Exists(".env")) Env.Load(".env");
        else if (File.Exists("../.env")) Env.Load("../.env");
    }

    // Get environment variable by key
    private static string? GetEnv(string key) =>
        Environment.GetEnvironmentVariable(key);

    // Build PostgreSQL connection string from environment variables
    private static string BuildConnectionString()
    {
        string host = GetEnv("DB_HOST") ?? "localhost";
        string port = GetEnv("DB_PORT") ?? "5432";
        string user = GetEnv("DB_USER") ?? throw new InvalidOperationException("DB_USER is not set");
        string password = GetEnv("DB_PASSWORD") ?? throw new InvalidOperationException("DB_PASSWORD is not set");
        string db = GetEnv("DB_NAME") ?? throw new InvalidOperationException("DB_NAME is not set");

        return $"Host={host};Port={port};Username={user};Password={password};Database={db}";
    }

    // Register all repositories
    private static void ConfigureRepositories(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        builder.Services.AddScoped<IRepository<TaskModel>, TaskRepository>();
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
        builder.Services.AddScoped<IProjectStatisticRepository, ProjectStatisticRepository>();
    }

    // Register all services
    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITaskService, TaskService>();
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IFavoriteService, FavoriteService>();
        builder.Services.AddScoped<IEntityTypeService, EntityTypeService>();
        builder.Services.AddScoped<IStatusService, StatusService>();
        builder.Services.AddScoped<ITaskHistoryService, TaskHistoryService>();
        builder.Services.AddScoped<IProjectMemberService, ProjectMemberService>();
        builder.Services.AddScoped<ITaskSearchService, TaskSearchService>();
    }

    #endregion
}
