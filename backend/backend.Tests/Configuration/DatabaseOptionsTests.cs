using backend.Configuration;

namespace backend.Tests.Configuration;

public class DatabaseOptionsTests
{
    [Fact]
    public void LoadFromEnvironment_ShouldIncludeTimezoneUtcInConnectionString()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DB_HOST", "localhost");
        Environment.SetEnvironmentVariable("DB_PORT", "5432");
        Environment.SetEnvironmentVariable("DB_USER", "testuser");
        Environment.SetEnvironmentVariable("DB_PASSWORD", "testpass");
        Environment.SetEnvironmentVariable("DB_NAME", "testdb");

        try
        {
            // Act
            var options = DatabaseOptions.LoadFromEnvironment();

            // Assert
            Assert.Contains("Timezone=UTC", options.ConnectionString);
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("DB_HOST", null);
            Environment.SetEnvironmentVariable("DB_PORT", null);
            Environment.SetEnvironmentVariable("DB_USER", null);
            Environment.SetEnvironmentVariable("DB_PASSWORD", null);
            Environment.SetEnvironmentVariable("DB_NAME", null);
        }
    }

    [Fact]
    public void LoadFromEnvironment_ShouldGenerateCorrectConnectionString()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DB_HOST", "db.example.com");
        Environment.SetEnvironmentVariable("DB_PORT", "5433");
        Environment.SetEnvironmentVariable("DB_USER", "myuser");
        Environment.SetEnvironmentVariable("DB_PASSWORD", "mypassword");
        Environment.SetEnvironmentVariable("DB_NAME", "mydb");

        try
        {
            // Act
            var options = DatabaseOptions.LoadFromEnvironment();

            // Assert
            var expectedConnectionString = "Host=db.example.com;Port=5433;Username=myuser;Password=mypassword;Database=mydb;Timezone=UTC";
            Assert.Equal(expectedConnectionString, options.ConnectionString);
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("DB_HOST", null);
            Environment.SetEnvironmentVariable("DB_PORT", null);
            Environment.SetEnvironmentVariable("DB_USER", null);
            Environment.SetEnvironmentVariable("DB_PASSWORD", null);
            Environment.SetEnvironmentVariable("DB_NAME", null);
        }
    }

    [Fact]
    public void LoadFromEnvironment_ShouldSetAllProperties()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DB_HOST", "myhost");
        Environment.SetEnvironmentVariable("DB_PORT", "5434");
        Environment.SetEnvironmentVariable("DB_USER", "user123");
        Environment.SetEnvironmentVariable("DB_PASSWORD", "pass456");
        Environment.SetEnvironmentVariable("DB_NAME", "database789");

        try
        {
            // Act
            var options = DatabaseOptions.LoadFromEnvironment();

            // Assert
            Assert.Equal("myhost", options.Host);
            Assert.Equal(5434, options.Port);
            Assert.Equal("user123", options.User);
            Assert.Equal("pass456", options.Password);
            Assert.Equal("database789", options.Database);
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("DB_HOST", null);
            Environment.SetEnvironmentVariable("DB_PORT", null);
            Environment.SetEnvironmentVariable("DB_USER", null);
            Environment.SetEnvironmentVariable("DB_PASSWORD", null);
            Environment.SetEnvironmentVariable("DB_NAME", null);
        }
    }

    [Fact]
    public void LoadFromEnvironment_WithDefaultHost_ShouldUseLocalhost()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DB_HOST", null); // Not set, should default to localhost
        Environment.SetEnvironmentVariable("DB_PORT", "5432");
        Environment.SetEnvironmentVariable("DB_USER", "testuser");
        Environment.SetEnvironmentVariable("DB_PASSWORD", "testpass");
        Environment.SetEnvironmentVariable("DB_NAME", "testdb");

        try
        {
            // Act
            var options = DatabaseOptions.LoadFromEnvironment();

            // Assert
            Assert.Equal("localhost", options.Host);
            Assert.Contains("Host=localhost", options.ConnectionString);
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("DB_PORT", null);
            Environment.SetEnvironmentVariable("DB_USER", null);
            Environment.SetEnvironmentVariable("DB_PASSWORD", null);
            Environment.SetEnvironmentVariable("DB_NAME", null);
        }
    }
}
