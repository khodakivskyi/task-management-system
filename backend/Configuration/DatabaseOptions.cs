namespace backend.Configuration;

/// <summary>
/// Database configuration loaded from environment variables
/// </summary>
public class DatabaseOptions
{
    public string Host { get; }
    public int Port { get; }
    public string User { get; }
    public string Password { get; }
    public string Database { get; }
    public string ConnectionString { get; }

    private DatabaseOptions(string host, int port, string user, string password, string database)
    {
        Host = host;
        Port = port;
        User = user;
        Password = password;
        Database = database;
        ConnectionString = $"Host={host};Port={port};Username={user};Password={password};Database={database};Timezone=UTC";
    }

    /// <summary>
    /// Load database options from environment variables
    /// </summary>
    public static DatabaseOptions LoadFromEnvironment()
    {
        var host = GetEnv("DB_HOST") ?? "localhost";
        var portStr = GetEnv("DB_PORT") ?? "5432";
        var user = GetEnvOrThrow("DB_USER");
        var password = GetEnvOrThrow("DB_PASSWORD");
        var database = GetEnvOrThrow("DB_NAME");

        if (!int.TryParse(portStr, out int port) || port <= 0 || port > 65535)
        {
            throw new InvalidOperationException($"DB_PORT must be a valid port number (1-65535), current: {portStr}");
        }

        return new DatabaseOptions(host, port, user, password, database);
    }

    private static string? GetEnv(string key) =>
        Environment.GetEnvironmentVariable(key);

    private static string GetEnvOrThrow(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Environment variable '{key}' is not set");
        }
        return value;
    }
}
