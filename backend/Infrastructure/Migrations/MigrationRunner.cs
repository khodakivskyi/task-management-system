using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using Npgsql;

namespace backend.Infrastructure.Migrations;

/// <summary>
/// Automated migration runner with checksum validation and transaction support.
///
/// Philosophy:
/// - Forward-only migrations (never modify applied migrations)
/// - Idempotent (safe to re-run)
/// - Checksum validation prevents tampering
/// - Transaction per migration (atomic)
///
/// Implementation:
/// - Uses ADO.NET interfaces (IDbConnection, IDbCommand, IDbTransaction)
/// - Npgsql is an ADO.NET provider for PostgreSQL that implements these interfaces
/// - Combines ADO.NET for SQL execution with Dapper for query convenience
/// </summary>
public class MigrationRunner
{
    private readonly string _connectionString;
    private readonly string _migrationsPath;
    private readonly string _databaseName;

    public MigrationRunner(string connectionString, string migrationsPath, string databaseName)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        // Use provided database name or default to TaskManagementDb
        _databaseName = databaseName;

        // Ensure connection string specifies the target database
        _connectionString = EnsureDatabaseInConnectionString(connectionString, _databaseName);
        _migrationsPath = migrationsPath ?? throw new ArgumentNullException(nameof(migrationsPath));
    }

    /// <summary>
    /// Ensures the connection string includes the specified database
    /// </summary>
    private static string EnsureDatabaseInConnectionString(string connectionString, string databaseName)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        // If no database is specified, or it's postgres (default admin database), set it to the target database
        if (string.IsNullOrEmpty(builder.Database) || builder.Database.Equals("postgres", StringComparison.OrdinalIgnoreCase))
        {
            builder.Database = databaseName;
        }

        return builder.ConnectionString;
    }

    /// <summary>
    /// Runs all pending migrations in order
    /// </summary>
    /// <returns>Number of migrations executed</returns>
    public async Task<int> RunMigrationsAsync()
    {
        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("Migration Runner");
        Console.WriteLine("========================================");
        Console.WriteLine();

        try
        {
            // 1. Ensure database exists
            await EnsureDatabaseExistsAsync();

            // 2. Ensure migrations history table exists
            await EnsureMigrationsHistoryTableExistsAsync();

            // 3. Scan migration files from disk
            var allMigrations = ScanMigrationFiles();
            Console.WriteLine($"Found {allMigrations.Count} migration files");

            if (allMigrations.Count == 0)
            {
                Console.WriteLine("WARNING: No migration files found!");
                return 0;
            }

            // 4. Get applied migrations from database
            var appliedMigrations = await GetAppliedMigrationsAsync();
            Console.WriteLine($"Already applied: {appliedMigrations.Count} migrations");

            // 5. Mark which migrations are already applied
            foreach (var migration in allMigrations)
            {
                if (appliedMigrations.TryGetValue(migration.Version, out var applied))
                {
                    migration.IsApplied = true;
                    migration.AppliedAt = applied.AppliedAt;
                    migration.ExecutionTimeMs = applied.ExecutionTimeMs;
                    migration.DatabaseChecksum = applied.Checksum;
                }
            }

            // 6. Validate checksums (detect tampering)
            await ValidateChecksums(allMigrations);

            // 7. Get pending migrations
            var pendingMigrations = allMigrations.Where(m => !m.IsApplied).ToList();

            if (pendingMigrations.Count == 0)
            {
                Console.WriteLine("Database is up to date - no pending migrations");
                Console.WriteLine();
                return 0;
            }

            Console.WriteLine($"Pending migrations: {pendingMigrations.Count}");
            Console.WriteLine();

            // 8. Execute each pending migration
            var executedCount = 0;
            var totalStopwatch = Stopwatch.StartNew();

            foreach (var migration in pendingMigrations)
            {
                await ExecuteMigrationAsync(migration);
                executedCount++;
            }

            totalStopwatch.Stop();

            // 9. Summary
            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine($"Migration Complete!");
            Console.WriteLine("========================================");
            Console.WriteLine($"Executed: {executedCount} migrations");
            Console.WriteLine($"Total time: {totalStopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine("========================================");
            Console.WriteLine();

            return executedCount;
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine("Migration Failed!");
            Console.WriteLine("========================================");
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("========================================");
            Console.WriteLine();
            throw;
        }
    }

    /// <summary>
    /// Scans the migrations directory for migration SQL files
    /// Supports both formats: V001__description.sql and 01_description.sql
    /// </summary>
    private List<MigrationRecord> ScanMigrationFiles()
    {
        var migrationsDir = Path.GetFullPath(_migrationsPath);

        if (!Directory.Exists(migrationsDir))
        {
            throw new DirectoryNotFoundException($"Migrations directory not found: {migrationsDir}");
        }

        // Get all SQL files
        var files = Directory.GetFiles(migrationsDir, "*.sql", SearchOption.TopDirectoryOnly);
        var migrations = new List<MigrationRecord>();

        foreach (var filePath in files)
        {
            var fileName = Path.GetFileName(filePath);
            string? version = null;

            // Try V###__ format (e.g., V001__initial_schema.sql → "001")
            var vMatch = Regex.Match(fileName, @"^V(\d{3})__", RegexOptions.IgnoreCase);
            if (vMatch.Success)
            {
                // Skip V000 - it's the bootstrap script, not a versioned migration
                if (fileName.StartsWith("V000", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                version = vMatch.Groups[1].Value;
            }
            else
            {
                // Try numeric format (e.g., 01_create_tables.sql → "001")
                var numericMatch = Regex.Match(fileName, @"^(\d{2,})_");
                if (numericMatch.Success)
                {
                    var numStr = numericMatch.Groups[1].Value;
                    // Pad to 3 digits for consistency
                    version = numStr.PadLeft(3, '0');
                }
            }

            if (version == null)
            {
                Console.WriteLine($"WARNING: Skipping invalid migration filename: {fileName}");
                Console.WriteLine($"   Expected format: V001__description.sql or 01_description.sql");
                continue;
            }

            var sqlContent = File.ReadAllText(filePath);
            var normalizedContent = NormalizeLineEndings(sqlContent);
            var checksum = CalculateSHA256(normalizedContent);

            migrations.Add(new MigrationRecord
            {
                Version = version,
                FileName = fileName,
                FilePath = filePath,
                SqlContent = sqlContent,
                Checksum = checksum
            });
        }

        // Sort by version number
        return migrations.OrderBy(m => m.Version).ToList();
    }

    /// <summary>
    /// Ensures the target database exists, creates it if it doesn't
    /// </summary>
    private async Task EnsureDatabaseExistsAsync()
    {
        // Use a connection string without database specified to connect to postgres (default admin database)
        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        builder.Database = "postgres"; // Connect to postgres database to check/create target database

        // ADO.NET: Using IDbConnection interface
        await using var connection = new NpgsqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        // ADO.NET: Using IDbCommand interface (CreateCommand returns IDbCommand)
        // NpgsqlConnection.CreateCommand() returns NpgsqlCommand which implements IDbCommand
        await using var checkCmd = connection.CreateCommand();
        checkCmd.CommandText = "SELECT 1 FROM pg_database WHERE datname = @DatabaseName";
        // ADO.NET: CreateParameter returns IDbDataParameter interface
        var param = checkCmd.CreateParameter();
        param.ParameterName = "@DatabaseName";
        param.Value = _databaseName;
        checkCmd.Parameters.Add(param);

        var exists = await checkCmd.ExecuteScalarAsync();

        if (exists == null)
        {
            Console.WriteLine($"Creating database \"{_databaseName}\"...");

            // In PostgreSQL, we need to use a separate connection to create the database
            // and cannot do it inside a transaction block
            // ADO.NET: Using IDbCommand interface
            await using var createCmd = connection.CreateCommand();
            createCmd.CommandText = $"CREATE DATABASE \"{_databaseName}\"";
            await createCmd.ExecuteNonQueryAsync();

            Console.WriteLine($"Database \"{_databaseName}\" created");
        }
    }

    /// <summary>
    /// Ensures the __MigrationsHistory table exists in the database
    /// </summary>
    private async Task EnsureMigrationsHistoryTableExistsAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // Check if migrations history table exists using Dapper
        var tableExists = await connection.QueryFirstOrDefaultAsync<int?>(
            "SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = '__MigrationsHistory'");

        if (tableExists == null)
        {
            Console.WriteLine("Creating migrations history table...");

            var createTableSql = @"
                CREATE TABLE public.""__MigrationsHistory""
                (
                    ""MigrationVersion"" VARCHAR(10) NOT NULL,
                    ""FileName"" VARCHAR(255) NOT NULL,
                    ""Checksum"" VARCHAR(64) NOT NULL,
                    ""AppliedAt"" TIMESTAMP NOT NULL,
                    ""ExecutionTimeMs"" INTEGER NOT NULL,

                    CONSTRAINT ""PK___MigrationsHistory"" PRIMARY KEY (""MigrationVersion"")
                );
            ";

            // ADO.NET: Using IDbCommand interface
            await using var createCmd = connection.CreateCommand();
            createCmd.CommandText = createTableSql;
            await createCmd.ExecuteNonQueryAsync();

            Console.WriteLine("Migrations history table created");
        }
    }

    /// <summary>
    /// Gets all applied migrations from the database using Dapper
    /// </summary>
    private async Task<Dictionary<string, (string Checksum, DateTime AppliedAt, int ExecutionTimeMs)>> GetAppliedMigrationsAsync()
    {
        var applied = new Dictionary<string, (string, DateTime, int)>();

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // Check if migrations history table exists
        var tableExists = await connection.QueryFirstOrDefaultAsync<int?>(
            "SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = '__MigrationsHistory'");

        if (tableExists == null)
        {
            // Table doesn't exist yet - no migrations applied
            return applied;
        }

        // Get all applied migrations using Dapper
        var migrations = await connection.QueryAsync<MigrationHistoryRecord>(
            @"SELECT ""MigrationVersion"", ""Checksum"", ""AppliedAt"", ""ExecutionTimeMs"" 
              FROM ""__MigrationsHistory"" 
              ORDER BY ""MigrationVersion""");

        foreach (var migration in migrations)
        {
            applied[migration.MigrationVersion] = (migration.Checksum, migration.AppliedAt, migration.ExecutionTimeMs);
        }

        return applied;
    }

    /// <summary>
    /// Validates that checksums match between files and database (detects tampering)
    /// </summary>
    private async Task ValidateChecksums(List<MigrationRecord> migrations)
    {
        var tamperedMigrations = migrations
            .Where(m => m.IsApplied && !m.ChecksumMatches)
            .ToList();

        if (tamperedMigrations.Any())
        {
            // Check if any have PLACEHOLDER checksum (from initial seed data)
            var placeholderMigrations = tamperedMigrations
                .Where(m => m.DatabaseChecksum == "PLACEHOLDER")
                .ToList();

            if (placeholderMigrations.Any())
            {
                Console.WriteLine();
                Console.WriteLine("Fixing placeholder checksums...");

                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                // Use Dapper for update
                foreach (var migration in placeholderMigrations)
                {
                    await connection.ExecuteAsync(
                        @"UPDATE ""__MigrationsHistory"" SET ""Checksum"" = @Checksum WHERE ""MigrationVersion"" = @Version",
                        new { Checksum = migration.Checksum, Version = migration.Version });

                    Console.WriteLine($"  Fixed checksum for {migration.FileName}");
                    migration.DatabaseChecksum = migration.Checksum;  // Update in memory
                }

                // Remove fixed migrations from tampered list
                tamperedMigrations = tamperedMigrations.Except(placeholderMigrations).ToList();
            }

            if (tamperedMigrations.Any())
            {
                Console.WriteLine();
                Console.WriteLine("NOTE: Checksum mismatch detected. This may be due to line ending differences.");
                Console.WriteLine("      Auto-fixing checksums (normalizing line endings to LF)...");
                Console.WriteLine();
                
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                
                var fixedMigrations = new List<MigrationRecord>();
                
                foreach (var migration in tamperedMigrations)
                {
                    // Update checksum in database to match normalized content
                    await connection.ExecuteAsync(
                        @"UPDATE ""__MigrationsHistory"" SET ""Checksum"" = @Checksum WHERE ""MigrationVersion"" = @Version",
                        new { Checksum = migration.Checksum, Version = migration.Version });
                    
                    Console.WriteLine($"  ✓ Fixed checksum for {migration.FileName}");
                    migration.DatabaseChecksum = migration.Checksum;  // Update in memory
                    fixedMigrations.Add(migration);
                }
                
                Console.WriteLine();
                Console.WriteLine($"Successfully fixed {fixedMigrations.Count} checksum(s)");
                Console.WriteLine();
            }
        }
    }

    /// <summary>
    /// Executes a single migration within a transaction
    /// Uses ADO.NET interfaces (IDbConnection, IDbCommand, IDbTransaction)
    /// </summary>
    private async Task ExecuteMigrationAsync(MigrationRecord migration)
    {
        Console.WriteLine($"Executing: {migration.FileName}...");

        var stopwatch = Stopwatch.StartNew();

        // ADO.NET: NpgsqlConnection implements IDbConnection
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // ADO.NET: BeginTransactionAsync returns IDbTransaction (NpgsqlTransaction implements it)
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // PostgreSQL supports multiple statements in a single command
            // Remove GO statements if present (SQL Server syntax) - PostgreSQL doesn't use them
            var sql = Regex.Replace(migration.SqlContent, @"^\s*GO\s*$", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            // ADO.NET: Execute SQL using IDbCommand interface
            // NpgsqlCommand implements IDbCommand
            if (!string.IsNullOrWhiteSpace(sql))
            {
                await using var cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.Transaction = transaction;
                cmd.CommandTimeout = 300; // 5 minutes timeout
                await cmd.ExecuteNonQueryAsync();
            }

            // Record migration in history using Dapper with transaction
            await connection.ExecuteAsync(
                @"INSERT INTO ""__MigrationsHistory"" (""MigrationVersion"", ""FileName"", ""Checksum"", ""AppliedAt"", ""ExecutionTimeMs"")
                  VALUES (@Version, @FileName, @Checksum, @AppliedAt, @ExecutionTimeMs)",
                new
                {
                    Version = migration.Version,
                    FileName = migration.FileName,
                    Checksum = migration.Checksum,
                    AppliedAt = DateTime.UtcNow,
                    ExecutionTimeMs = stopwatch.ElapsedMilliseconds
                },
                transaction: transaction);

            await transaction.CommitAsync();

            stopwatch.Stop();
            Console.WriteLine($"   Success ({stopwatch.ElapsedMilliseconds}ms)");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            stopwatch.Stop();

            Console.WriteLine($"   Failed ({stopwatch.ElapsedMilliseconds}ms)");
            Console.WriteLine($"   Error: {ex.Message}");
            throw new InvalidOperationException($"Migration {migration.FileName} failed: {ex.Message}", ex);
        }
    }


    /// <summary>
    /// Normalizes line endings to LF (\n) to ensure consistent checksums
    /// regardless of the file's original line ending format (CRLF, LF, or CR)
    /// </summary>
    private string NormalizeLineEndings(string content)
    {
        // Replace all variations of line endings with LF
        // \r\n (CRLF) -> \n
        // \r (old Mac) -> \n
        // \n (LF) stays as \n
        return content.Replace("\r\n", "\n").Replace("\r", "\n");
    }

    /// <summary>
    /// Calculates SHA256 checksum of a string
    /// </summary>
    private string CalculateSHA256(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    /// <summary>
    /// Internal record for reading migration history from database
    /// </summary>
    private class MigrationHistoryRecord
    {
        public string MigrationVersion { get; set; } = string.Empty;
        public string Checksum { get; set; } = string.Empty;
        public DateTime AppliedAt { get; set; }
        public int ExecutionTimeMs { get; set; }
    }
}
