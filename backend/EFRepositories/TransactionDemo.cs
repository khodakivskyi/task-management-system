using backend.EFModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System.Data.Common;

namespace backend.EFRepositories;

/// <summary>
/// Demonstrates transaction management in Entity Framework Core:
/// - Implicit transactions (automatic)
/// - Explicit transactions (BeginTransaction)
/// - Integration with external ADO.NET transactions
/// </summary>
public class TransactionDemo
{
    private readonly TaskManagementDbContext _context;
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly string _connectionString;

    public TransactionDemo(
        TaskManagementDbContext context,
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        string connectionString)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    /// Demonstrates implicit transactions (automatic)
    /// SaveChangesAsync() automatically creates a transaction
    /// Shows Add/Update/Remove operations in single transaction
    /// </summary>
    public async Task DemonstrateImplicitTransactionAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Implicit Transactions (Automatic) ===\n");

        Console.WriteLine("SaveChangesAsync() automatically creates a transaction");
        Console.WriteLine("All changes (Add/Update/Remove) in a single SaveChangesAsync() call are atomic");
        Console.WriteLine("If error occurs - none of the changes are applied\n");

        try
        {
            // Get existing user and status
            var users = await _userRepository.GetAllAsync(trackChanges: false, cancellationToken);
            var user = users.FirstOrDefault();
            if (user == null)
            {
                Console.WriteLine("No users found. Cannot demonstrate transaction.\n");
                return;
            }

            var statuses = await _context.Set<Status>().AsNoTracking().ToListAsync(cancellationToken);
            var status = statuses.FirstOrDefault();
            if (status == null)
            {
                Console.WriteLine("No statuses found. Cannot demonstrate transaction.\n");
                return;
            }

            Console.WriteLine("Performing multiple operations:");
            Console.WriteLine("  1. Add - Creating 2 new tasks");
            Console.WriteLine("  2. Update - Updating existing task (if any)");
            Console.WriteLine("  3. Remove - Deleting a task (if any)\n");

            // ADD operations
            var task1 = new TaskModel
            {
                OwnerId = user.Id,
                StatusId = status.Id,
                Title = "Task 1 (Implicit Transaction - Add)",
                Description = "First task added in transaction",
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                EstimatedHours = 1
            };

            var task2 = new TaskModel
            {
                OwnerId = user.Id,
                StatusId = status.Id,
                Title = "Task 2 (Implicit Transaction - Add)",
                Description = "Second task added in transaction",
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                EstimatedHours = 2
            };

            _context.Set<TaskModel>().Add(task1);
            _context.Set<TaskModel>().Add(task2);
            Console.WriteLine("  SUCCESS: 2 tasks added to context");

            // UPDATE operation - update existing task if available
            var existingTasks = await _context.Set<TaskModel>()
                .Take(1)
                .ToListAsync(cancellationToken);
            
            if (existingTasks.Any())
            {
                var taskToUpdate = existingTasks.First();
                taskToUpdate.Title = "Updated Task (Implicit Transaction - Update)";
                taskToUpdate.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                Console.WriteLine($"  SUCCESS: 1 task marked for update (ID: {taskToUpdate.Id})");
            }

            // REMOVE operation - delete a task if available
            var tasksToDelete = await _context.Set<TaskModel>()
                .Where(t => t.Title.Contains("Demo") || t.Title.Contains("Test"))
                .Take(1)
                .ToListAsync(cancellationToken);
            
            if (tasksToDelete.Any())
            {
                var taskToDelete = tasksToDelete.First();
                _context.Set<TaskModel>().Remove(taskToDelete);
                Console.WriteLine($"  SUCCESS: 1 task marked for deletion (ID: {taskToDelete.Id})");
            }

            // Single SaveChangesAsync() - all changes (Add/Update/Remove) are in one implicit transaction
            Console.WriteLine("\n  Calling SaveChangesAsync() - all operations execute atomically...");
            var savedCount = await _context.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"  SUCCESS: Saved {savedCount} changes in one implicit transaction");
            Console.WriteLine("  SUCCESS: All operations committed atomically\n");
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"✗ DbUpdateException: {dbEx.Message}");
            Console.WriteLine("  - Transaction was automatically rolled back");
            Console.WriteLine("  - None of the changes were applied\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error: {ex.Message}");
            Console.WriteLine("  - Transaction was automatically rolled back\n");
        }

        Console.WriteLine("=== Implicit Transaction Demo Completed ===\n");
    }

    /// <summary>
    /// Demonstrates explicit transactions (BeginTransaction)
    /// Manual control over transaction commit/rollback
    /// Use case: Multiple independent SaveChangesAsync() calls in one transaction
    /// </summary>
    public async Task DemonstrateExplicitTransactionAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Explicit Transactions (BeginTransaction) ===\n");

        Console.WriteLine("Use case: When you need multiple independent SaveChangesAsync() calls");
        Console.WriteLine("in a single transaction (e.g., complex business operations)\n");

        IDbContextTransaction? transaction = null;

        try
        {
            // Get existing user and status
            var users = await _userRepository.GetAllAsync(trackChanges: false, cancellationToken);
            var user = users.FirstOrDefault();
            if (user == null)
            {
                Console.WriteLine("No users found. Cannot demonstrate transaction.\n");
                return;
            }

            var statuses = await _context.Set<Status>().AsNoTracking().ToListAsync(cancellationToken);
            var status = statuses.FirstOrDefault();
            if (status == null)
            {
                Console.WriteLine("No statuses found. Cannot demonstrate transaction.\n");
                return;
            }

            // Begin explicit transaction
            transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            Console.WriteLine("SUCCESS: Transaction started\n");

            // First SaveChangesAsync() - Create task 1
            Console.WriteLine("Step 1: Creating first task...");
            var task1 = new TaskModel
            {
                OwnerId = user.Id,
                StatusId = status.Id,
                Title = "Task 1 (Explicit Transaction)",
                Description = "First task in explicit transaction",
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                EstimatedHours = 1
            };

            _context.Set<TaskModel>().Add(task1);
            await _context.SaveChangesAsync(cancellationToken);
            Console.WriteLine("  SUCCESS: Task 1 saved (not yet committed to database)");

            // Second SaveChangesAsync() - Create task 2
            Console.WriteLine("Step 2: Creating second task...");
            var task2 = new TaskModel
            {
                OwnerId = user.Id,
                StatusId = status.Id,
                Title = "Task 2 (Explicit Transaction)",
                Description = "Second task in explicit transaction",
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                EstimatedHours = 2
            };

            _context.Set<TaskModel>().Add(task2);
            await _context.SaveChangesAsync(cancellationToken);
            Console.WriteLine("  SUCCESS: Task 2 saved (not yet committed to database)");

            // Third SaveChangesAsync() - Update task 1
            Console.WriteLine("Step 3: Updating first task...");
            task1.Title = "Task 1 (Updated in Transaction)";
            task1.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            await _context.SaveChangesAsync(cancellationToken);
            Console.WriteLine("  SUCCESS: Task 1 updated (not yet committed to database)");

            // Commit transaction - all changes are now persisted
            await transaction.CommitAsync(cancellationToken);
            Console.WriteLine("\nSUCCESS: Transaction committed - all 3 operations are now persisted");
            Console.WriteLine("  - Both tasks created");
            Console.WriteLine("  - Task 1 updated");
            Console.WriteLine("  - All in one atomic transaction\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nERROR: Error occurred: {ex.Message}");
            if (transaction != null)
            {
                await transaction.RollbackAsync(cancellationToken);
                Console.WriteLine("SUCCESS: Transaction rolled back - no changes persisted\n");
            }
        }
        finally
        {
            if (transaction != null)
            {
                await transaction.DisposeAsync();
            }
        }

        Console.WriteLine("=== Explicit Transaction Demo Completed ===\n");
    }

    /// <summary>
    /// Demonstrates transaction rollback on error
    /// </summary>
    public async Task DemonstrateTransactionRollbackAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Transaction Rollback on Error ===\n");

        Console.WriteLine("Demonstrating automatic rollback when error occurs\n");

        IDbContextTransaction? transaction = null;

        try
        {
            // Get existing user and status
            var users = await _userRepository.GetAllAsync(trackChanges: false, cancellationToken);
            var user = users.FirstOrDefault();
            if (user == null)
            {
                Console.WriteLine("No users found. Cannot demonstrate transaction.\n");
                return;
            }

            var statuses = await _context.Set<Status>().AsNoTracking().ToListAsync(cancellationToken);
            var status = statuses.FirstOrDefault();
            if (status == null)
            {
                Console.WriteLine("No statuses found. Cannot demonstrate transaction.\n");
                return;
            }

            transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            Console.WriteLine("SUCCESS: Transaction started\n");

            // Create valid task
            var validTask = new TaskModel
            {
                OwnerId = user.Id,
                StatusId = status.Id,
                Title = "Valid Task (Will be rolled back)",
                Description = "This task will be rolled back due to error",
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                EstimatedHours = 1
            };

            _context.Set<TaskModel>().Add(validTask);
            await _context.SaveChangesAsync(cancellationToken);
            Console.WriteLine("SUCCESS: Valid task saved");

            // Try to create invalid task (will cause error)
            var invalidTask = new TaskModel
            {
                OwnerId = 99999, // Invalid foreign key - will cause error
                StatusId = status.Id,
                Title = "Invalid Task",
                Description = "This will cause foreign key violation",
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                EstimatedHours = 1
            };

            _context.Set<TaskModel>().Add(invalidTask);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: Error occurred: {ex.Message}");
            if (transaction != null)
            {
                await transaction.RollbackAsync(cancellationToken);
                Console.WriteLine("SUCCESS: Transaction rolled back - valid task was also rolled back\n");
            }
        }
        finally
        {
            if (transaction != null)
            {
                await transaction.DisposeAsync();
            }
        }

        Console.WriteLine("=== Transaction Rollback Demo Completed ===\n");
    }

    /// <summary>
    /// Demonstrates SaveChanges internals
    /// Shows how EF Core tracks changes and generates SQL
    /// </summary>
    public async Task DemonstrateSaveChangesInternalsAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== SaveChanges Internals ===\n");

        Console.WriteLine("Understanding SaveChanges() behavior:");
        Console.WriteLine("  1. EF Core tracks all changes to entities");
        Console.WriteLine("  2. SaveChanges() creates an implicit transaction");
        Console.WriteLine("  3. Generates SQL for INSERT/UPDATE/DELETE");
        Console.WriteLine("  4. Executes SQL in transaction");
        Console.WriteLine("  5. Commits transaction if successful");
        Console.WriteLine("  6. Rolls back on error\n");

        try
        {
            var users = await _userRepository.GetAllAsync(trackChanges: false, cancellationToken);
            var user = users.FirstOrDefault();
            if (user == null)
            {
                Console.WriteLine("No users found.\n");
                return;
            }

            var statuses = await _context.Set<Status>().AsNoTracking().ToListAsync(cancellationToken);
            var status = statuses.FirstOrDefault();
            if (status == null)
            {
                Console.WriteLine("No statuses found.\n");
                return;
            }

            // Create new entity
            var newTask = new TaskModel
            {
                OwnerId = user.Id,
                StatusId = status.Id,
                Title = "Task for SaveChanges Demo",
                Description = "Demonstrating SaveChanges internals",
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                EstimatedHours = 5
            };

            _context.Set<TaskModel>().Add(newTask);
            Console.WriteLine("SUCCESS: Entity added to context (state: Added)");

            // SaveChanges will:
            // 1. Detect entity is in Added state
            // 2. Generate INSERT SQL
            // 3. Execute in transaction
            var count = await _context.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"SUCCESS: SaveChanges() executed - {count} entity saved");
            Console.WriteLine("  - Generated INSERT SQL");
            Console.WriteLine("  - Executed in implicit transaction");
            Console.WriteLine("  - Transaction committed\n");

            // Update entity
            newTask.Title = "Updated Task Title";
            newTask.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            Console.WriteLine("SUCCESS: Entity modified (state: Modified)");

            count = await _context.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"SUCCESS: SaveChanges() executed - {count} entity updated");
            Console.WriteLine("  - Generated UPDATE SQL");
            Console.WriteLine("  - Executed in implicit transaction");
            Console.WriteLine("  - Transaction committed\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}\n");
        }

        Console.WriteLine("=== SaveChanges Internals Demo Completed ===\n");
    }

    /// <summary>
    /// Demonstrates integration with external ADO.NET transaction
    /// Creates NpgsqlConnection and NpgsqlTransaction, binds EF Core to it via UseTransaction()
    /// </summary>
    public async Task DemonstrateAdoNetTransactionIntegrationAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== ADO.NET Transaction Integration ===\n");

        Console.WriteLine("Use case: Integrating EF Core with existing ADO.NET code");
        Console.WriteLine("Both EF Core and ADO.NET operations work in the same transaction\n");

        NpgsqlConnection? connection = null;
        NpgsqlTransaction? adoTransaction = null;

        try
        {
            // Get existing user and status
            var users = await _userRepository.GetAllAsync(trackChanges: false, cancellationToken);
            var user = users.FirstOrDefault();
            if (user == null)
            {
                Console.WriteLine("No users found. Cannot demonstrate transaction.\n");
                return;
            }

            var statuses = await _context.Set<Status>().AsNoTracking().ToListAsync(cancellationToken);
            var status = statuses.FirstOrDefault();
            if (status == null)
            {
                Console.WriteLine("No statuses found. Cannot demonstrate transaction.\n");
                return;
            }

            // Step 1: Create ADO.NET connection and transaction
            connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            adoTransaction = await connection.BeginTransactionAsync(cancellationToken);
            Console.WriteLine("SUCCESS: ADO.NET connection opened and transaction started\n");

            // Step 2: Bind EF Core DbContext to existing ADO.NET transaction
            await _context.Database.UseTransactionAsync(adoTransaction, cancellationToken);
            Console.WriteLine("SUCCESS: EF Core DbContext bound to ADO.NET transaction\n");

            // Step 3: Perform operation via EF Core
            Console.WriteLine("Step 1: Creating task via EF Core...");
            var task = new TaskModel
            {
                OwnerId = user.Id,
                StatusId = status.Id,
                Title = "Task via EF Core (ADO.NET Transaction)",
                Description = "Created using EF Core in ADO.NET transaction",
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                EstimatedHours = 5
            };

            _context.Set<TaskModel>().Add(task);
            await _context.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"  SUCCESS: Task created via EF Core (ID: {task.Id})\n");

            // Step 4: Perform operation via ADO.NET (raw SQL)
            Console.WriteLine("Step 2: Executing raw SQL via ADO.NET...");
            using var command = new NpgsqlCommand(
                "INSERT INTO \"Tasks\" (\"OwnerId\", \"StatusId\", \"Title\", \"Description\", \"CreatedAt\", \"UpdatedAt\", \"EstimatedHours\", \"ActualHours\") " +
                "VALUES (@ownerId, @statusId, @title, @description, @createdAt, @updatedAt, @estimatedHours, @actualHours) " +
                "RETURNING \"Id\"",
                connection,
                adoTransaction);

            command.Parameters.AddWithValue("@ownerId", user.Id);
            command.Parameters.AddWithValue("@statusId", status.Id);
            command.Parameters.AddWithValue("@title", "Task via ADO.NET (Same Transaction)");
            command.Parameters.AddWithValue("@description", "Created using raw SQL in same transaction");
            command.Parameters.AddWithValue("@createdAt", DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified));
            command.Parameters.AddWithValue("@updatedAt", DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified));
            command.Parameters.AddWithValue("@estimatedHours", 3);
            command.Parameters.AddWithValue("@actualHours", 0);

            var taskId = await command.ExecuteScalarAsync(cancellationToken);
            Console.WriteLine($"  SUCCESS: Task created via ADO.NET (ID: {taskId})\n");

            // Step 5: Commit transaction (both operations are committed together)
            await adoTransaction.CommitAsync(cancellationToken);
            Console.WriteLine("SUCCESS: Transaction committed via ADO.NET");
            Console.WriteLine("  - Both EF Core and ADO.NET operations are persisted");
            Console.WriteLine("  - All changes are atomic\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nERROR: Error occurred: {ex.Message}");
            if (adoTransaction != null)
            {
                await adoTransaction.RollbackAsync(cancellationToken);
                Console.WriteLine("SUCCESS: Transaction rolled back via ADO.NET - no changes persisted\n");
            }
        }
        finally
        {
            if (adoTransaction != null)
            {
                await adoTransaction.DisposeAsync();
            }
            if (connection != null)
            {
                await connection.CloseAsync();
                await connection.DisposeAsync();
            }
        }

        Console.WriteLine("=== ADO.NET Transaction Integration Demo Completed ===\n");
    }

    /// <summary>
    /// Demonstrates exception handling for EF Core operations
    /// DbUpdateException - constraint violations, conflicts
    /// DbUpdateConcurrencyException - concurrency conflicts
    /// </summary>
    public async Task DemonstrateExceptionHandlingAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Exception Handling ===\n");

        Console.WriteLine("Demonstrating proper exception handling:");
        Console.WriteLine("  - DbUpdateException: constraint violations, foreign key violations");
        Console.WriteLine("  - DbUpdateConcurrencyException: concurrency conflicts\n");

        // Test 1: DbUpdateException - Foreign key violation
        Console.WriteLine("Test 1: DbUpdateException - Foreign Key Violation");
        try
        {
            var invalidTask = new TaskModel
            {
                OwnerId = 99999, // Invalid foreign key
                StatusId = 99999, // Invalid foreign key
                Title = "Invalid Task",
                Description = "This will cause foreign key violation",
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                EstimatedHours = 1
            };

            _context.Set<TaskModel>().Add(invalidTask);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"  SUCCESS: Caught DbUpdateException: {dbEx.Message}");
            Console.WriteLine("  - This exception occurs for constraint violations");
            Console.WriteLine("  - Foreign key violations, unique constraint violations, etc.");
            Console.WriteLine("  - Transaction was automatically rolled back\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ERROR: Unexpected exception: {ex.Message}\n");
        }

        // Test 2: DbUpdateException - Unique constraint violation (if exists)
        Console.WriteLine("Test 2: DbUpdateException - Constraint Violation");
        try
        {
            var users = await _userRepository.GetAllAsync(trackChanges: false, cancellationToken);
            var existingUser = users.FirstOrDefault();
            if (existingUser != null)
            {
                // Try to create user with duplicate login (if unique constraint exists)
                var duplicateUser = new User
                {
                    Name = "Duplicate",
                    Surname = "User",
                    Login = existingUser.Login, // Duplicate login
                    PasswordHash = "hash",
                    Salt = "salt",
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                _context.Set<User>().Add(duplicateUser);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"  SUCCESS: Caught DbUpdateException: {dbEx.Message}");
            Console.WriteLine("  - This exception occurs for unique constraint violations");
            Console.WriteLine("  - Transaction was automatically rolled back\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  - No constraint violation (or different error): {ex.GetType().Name}\n");
        }

        // Test 3: DbUpdateConcurrencyException - Concurrency conflict
        Console.WriteLine("Test 3: DbUpdateConcurrencyException - Concurrency Conflict");
        try
        {
            var tasks = await _context.Set<TaskModel>()
                .Take(1)
                .ToListAsync(cancellationToken);

            if (tasks.Any())
            {
                var task = tasks.First();

                // Simulate concurrent update by modifying row version/timestamp
                // Note: This requires a RowVersion or Timestamp column in the entity
                // For this demo, we'll show the concept even if the exception doesn't occur

                // Load task in first context
                var task1 = await _context.Set<TaskModel>()
                    .FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken);

                if (task1 != null)
                {
                    // Simulate: another user updates the task (in real scenario, this would be another DbContext)
                    // For demo purposes, we'll update it directly to show the concept
                    task1.Title = "Updated by User 1";
                    await _context.SaveChangesAsync(cancellationToken);

                    // Try to update with stale data (would cause concurrency exception if RowVersion exists)
                    // In real scenario, this would be from another DbContext instance
                    Console.WriteLine("  - Concurrency conflict simulation");
                    Console.WriteLine("  - In real scenario with RowVersion/Timestamp, this would throw DbUpdateConcurrencyException");
                    Console.WriteLine("  - Current schema doesn't have concurrency tokens, so exception won't occur\n");
                }
            }
            else
            {
                Console.WriteLine("  - No tasks found for concurrency test\n");
            }
        }
        catch (DbUpdateConcurrencyException concurrencyEx)
        {
            Console.WriteLine($"  SUCCESS: Caught DbUpdateConcurrencyException: {concurrencyEx.Message}");
            Console.WriteLine("  - This exception occurs when entity was modified by another user");
            Console.WriteLine("  - Requires RowVersion or Timestamp column in entity");
            Console.WriteLine("  - Transaction was automatically rolled back\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  - Different exception or no concurrency token: {ex.GetType().Name}\n");
        }

        Console.WriteLine("=== Exception Handling Demo Completed ===\n");
    }

    /// <summary>
    /// Runs all transaction demonstrations
    /// </summary>
    public async Task RunAllDemonstrationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await DemonstrateImplicitTransactionAsync(cancellationToken);
            await DemonstrateExplicitTransactionAsync(cancellationToken);
            await DemonstrateTransactionRollbackAsync(cancellationToken);
            await DemonstrateAdoNetTransactionIntegrationAsync(cancellationToken);
            await DemonstrateExceptionHandlingAsync(cancellationToken);
            await DemonstrateSaveChangesInternalsAsync(cancellationToken);

            Console.WriteLine("=== All Transaction Demonstrations Completed ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during demonstration: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}

