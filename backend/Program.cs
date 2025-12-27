using backend.GraphQL;
using backend.Infrastructure.Migrations;
using DotNetEnv;
using GraphQL.Execution;

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

        builder.Services.AddSingleton(new MigrationRunner(connectionString, "Migrations", dbName));

        builder.Services.AddSingleton(connectionString);

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

        // Use MigrationRunner while starting the app
        using (var scope = app.Services.CreateScope())
        {
            var runner = scope.ServiceProvider.GetRequiredService<MigrationRunner>();
            await runner.RunMigrationsAsync();
        }

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
