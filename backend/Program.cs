using System.Text;
using backend.Configuration;
using backend.GraphQL;
using backend.GraphQL.Extensions;
using backend.Infrastructure.Migrations;
using backend.Infrastructure.Repositories;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services;
using backend.Services.Interfaces;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace backend;

public static partial class Program
{
    private static async Task Main(string[] args)
    {
        // Load .env file
        ConfigurationLoader.LoadEnvironmentFile();

        // Load all configurations
        var (dbConfig, jwtConfig, appConfig) = ConfigurationLoader.LoadAll();

        var builder = WebApplication.CreateBuilder(args);

        // Configure URLs
        builder.WebHost.UseUrls($"http://0.0.0.0:{appConfig.Port}");

        // Register configurations as singletons
        builder.Services.AddSingleton(dbConfig);
        builder.Services.AddSingleton(jwtConfig);
        builder.Services.AddSingleton(appConfig);

        // Register connection string and migration runner
        builder.Services.AddSingleton(dbConfig.ConnectionString);
        builder.Services.AddSingleton(new MigrationRunner(dbConfig.ConnectionString, "Migrations/Scripts", dbConfig.Database));

        // Register repositories
        ConfigureRepositories(builder);

        // Register services
        ConfigureServices(builder);

        // Configure JWT Authentication
        ConfigureJwtAuthentication(builder, jwtConfig);

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
        Console.WriteLine($"GraphQL UI: http://localhost:{appConfig.Port}/graphql");

        app.MapGraphQL();
        await app.RunAsync();
    }

    #region Helpers
    // Register all repositories
    private static void ConfigureRepositories(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        builder.Services.AddScoped<IRepository<TaskModel>, TaskRepository>();
        builder.Services.AddScoped<IRepository<Project>, ProjectRepository>();
        builder.Services.AddScoped<IRepository<Category>, CategoryRepository>();
        builder.Services.AddScoped<IRepository<Comment>, CommentRepository>();
        builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
        builder.Services.AddScoped<IRepository<EntityType>, EntityTypeRepository>();
        builder.Services.AddScoped<IRepository<Status>, StatusRepository>();
        builder.Services.AddScoped<ITaskHistoryRepository, TaskHistoryRepository>();
        builder.Services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
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
        builder.Services.AddScoped<IProjectStatisticsService, ProjectStatisticsService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
    }
    #endregion


    #region Configuration
    private static void ConfigureJwtAuthentication(WebApplicationBuilder builder, JwtOptions jwtConfig)
    {
        var key = Encoding.UTF8.GetBytes(jwtConfig.Secret);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtConfig.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddAuthorization();
    }
    #endregion
}
