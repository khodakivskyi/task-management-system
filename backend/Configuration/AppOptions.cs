namespace backend.Configuration;

/// <summary>
/// Application-level configuration
/// </summary>
public class AppOptions
{
    public int Port { get; }
    public string EnvironmentName { get; }

    private AppOptions(int port, string environmentName)
    {
        Port = port;
        EnvironmentName = environmentName;
    }

    /// <summary>
    /// Load application options from environment variables
    /// </summary>
    public static AppOptions LoadFromEnvironment()
    {
        var portStr = GetEnv("BACKEND_PORT") ?? "5000";
        var environmentName = GetEnv("ASPNETCORE_ENVIRONMENT") ?? "Development";

        if (!int.TryParse(portStr, out int port) || port <= 0 || port > 65535)
        {
            throw new InvalidOperationException($"BACKEND_PORT must be a valid port number (1-65535), current: {portStr}");
        }

        return new AppOptions(port, environmentName);
    }

    private static string? GetEnv(string key) =>
           Environment.GetEnvironmentVariable(key);
}