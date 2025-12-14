using backend.CodeFirst;
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

        // Register Code-First DbContext (created from scratch)
        // Only Code-First approach is used in this application
        builder.Services.AddDbContext<TaskManagementCodeFirstDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Register Code-First Demo
        builder.Services.AddScoped<CodeFirstDemo>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //builder.Services.AddAuthorization();
        //builder.Services.AddAuthentication();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            
            // Demonstrate Code-First DbContext
            var codeFirstDemo = scope.ServiceProvider.GetRequiredService<CodeFirstDemo>();
            await codeFirstDemo.RunDemonstrationAsync(cts.Token);
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