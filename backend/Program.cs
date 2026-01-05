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
        if (File.Exists(".env"))
        {
            Env.Load(".env");
        }
        else if (File.Exists(".. /.env"))
        {
            Env.Load("../.env");
        }

        var builder = WebApplication.CreateBuilder(args);

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

        builder.Services.AddSingleton(new MigrationRunner(connectionString, "Migrations/Scripts", dbName));

        builder.Services.AddSingleton(connectionString);

        // Register Repositories
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
        builder.Services.AddScoped<ITaskSearchService, TaskSearchService>();

        // GraphQL
        builder.Services.AddGraphQLServer()
            .AddQueryType<RootQuery>()
            .AddMutationType<RootMutation>()
            .AddErrorFilter<GraphQLErrorFilter>();

        // Logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        var app = builder.Build();

        // Run migrations with retry logic
        await app.RunMigrationsWithRetryAsync();

        Console.WriteLine("App started!");
        Console.WriteLine($"GraphQl UI: http://localhost:{Environment.GetEnvironmentVariable("BACKEND_PORT") ?? "5000"}/graphql");

        app.MapGraphQL();

        app.Run();
    }
}
