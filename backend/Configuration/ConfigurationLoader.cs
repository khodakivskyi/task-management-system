using DotNetEnv;

namespace backend.Configuration;

/// <summary>
/// Centralized configuration loader
/// </summary>
public static class ConfigurationLoader
{
    public static void LoadEnvironmentFile()
    {
        if (File.Exists(".env"))
        {
            Env.Load(".env");
            Console.WriteLine("SUCCESS: Loaded .env file from current directory");
        }
        else if (File.Exists("../.env"))
        {
            Env.Load("../.env");
            Console.WriteLine("SUCCESS: Loaded .env file from parent directory");
        }
        else
        {
            Console.WriteLine("FAIL: No .env file found, using system environment variables");
        }
    }

    /// <summary>
    /// Load all application configurations
    /// Validates all required settings on startup
    /// </summary>
    public static (DatabaseOptions database, JwtOptions jwt, AppOptions app) LoadAll()
    {
        Console.WriteLine("Loading configuration from environment variables...");

        DatabaseOptions database;
        JwtOptions jwt;
        AppOptions app;

        try
        {
            database = DatabaseOptions.LoadFromEnvironment();
            Console.WriteLine($"SUCCESS Database: {database.Host}:{database.Port}/{database.Database}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"FAIL: Failed to load database configuration: {ex.Message}", ex);
        }

        try
        {
            jwt = JwtOptions.LoadFromEnvironment();
            Console.WriteLine($"SUCCESS JWT: Issuer={jwt.Issuer}, Expiration={jwt.ExpirationHours}h");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"FAIL: Failed to load JWT configuration: {ex.Message}", ex);
        }

        try
        {
            app = AppOptions.LoadFromEnvironment();
            Console.WriteLine($"SUCCESS App: Port={app.Port}, EnvironmentName={app.EnvironmentName}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"FAIL: Failed to load app configuration: {ex.Message}", ex);
        }

        Console.WriteLine("SUCCESS: All configurations loaded successfully\n");

        return (database, jwt, app);
    }
}