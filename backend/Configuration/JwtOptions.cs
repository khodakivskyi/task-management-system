namespace backend.Configuration;

/// <summary>
/// JWT configuration loaded from .env
/// </summary>
public class JwtOptions
{
    public string Secret { get; }
    public string Issuer { get; }
    public string Audience { get; }
    public int ExpirationHours { get; }

    private JwtOptions(string secret, string issuer, string audience, int expirationHours)
    {
        Secret = secret;
        Issuer = issuer;
        Audience = audience;
        ExpirationHours = expirationHours;
    }

    /// <summary>
    /// Load JWT options from environment variables
    /// Throws exception if required variables are missing or invalid
    /// </summary>
    public static JwtOptions LoadFromEnvironment()
    {
        var secret = GetEnvOrThrow("JWT_SECRET");
        var issuer = GetEnv("JWT_ISSUER") ?? "TaskManagementSystem";
        var audience = GetEnv("JWT_AUDIENCE") ?? "TaskManagementSystem";
        var expirationHoursStr = GetEnv("JWT_EXPIRATION_HOURS") ?? "24";

        // Validate secret
        if (secret.Length < 32)
        {
            throw new InvalidOperationException($"JWT_SECRET must be at least 32 characters long (current: {secret.Length})");
        }

        // Parse expiration hours
        if (!int.TryParse(expirationHoursStr, out int expirationHours) || expirationHours < 1)
        {
            throw new InvalidOperationException($"JWT_EXPIRATION_HOURS must be a positive integer (current:  {expirationHoursStr})");
        }

        return new JwtOptions(secret, issuer, audience, expirationHours);
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