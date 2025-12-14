using backend;
using backend.EFRepositories;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        Env.Load();
        var builder = WebApplication.CreateBuilder(args);
        var env = builder.Environment;

        // Getting .env variables for db
        string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
            ?? throw new InvalidOperationException("DB_CONNECTION_STRING is not set");
        string dbName = Environment.GetEnvironmentVariable("DB_NAME")
            ?? throw new InvalidOperationException("DB_NAME is not set");

        builder.Services.AddSingleton(connectionString);

        // Register DbContext
        builder.Services.AddDbContext<TaskManagementDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Register EF Core Repositories
        builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //builder.Services.AddAuthorization();
        //builder.Services.AddAuthentication();

        var app = builder.Build();

        // Use MigrationRunner while starting the app
        // Enable Swagger UI for testing
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();
        //app.UseAuthentication();
        //app.UseAuthorization();

        app.Run();
    }
}