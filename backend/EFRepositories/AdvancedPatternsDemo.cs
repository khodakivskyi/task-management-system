using backend.EFModels;
using backend.EFModels.Enums;
using backend.EFModels.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace backend.EFRepositories;

/// <summary>
/// Demonstrates advanced EF Core patterns:
/// - Global Query Filters for Soft Delete
/// - Value Converters (Enum to String, JSON serialization)
/// - Owned Entities (Value Objects)
/// </summary>
public class AdvancedPatternsDemo
{
    private readonly TaskManagementDbContext _context;

    public AdvancedPatternsDemo(TaskManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Runs all advanced patterns demonstrations
    /// </summary>
    public async Task RunAllDemonstrationsAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║     Advanced EF Core Patterns Demonstration                    ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        await DemonstrateSoftDeleteAsync(cancellationToken);
        await DemonstrateValueConvertersAsync(cancellationToken);
        await DemonstrateOwnedEntitiesAsync(cancellationToken);

        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║     All Advanced Patterns Demonstrations Completed            ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");
    }

    /// <summary>
    /// Demonstrates Global Query Filter for soft delete
    /// Shows automatic filtering and IgnoreQueryFilters()
    /// </summary>
    public async Task DemonstrateSoftDeleteAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Global Query Filter: Soft Delete ===\n");

        try
        {
            // Get existing tasks
            var allTasks = await _context.Tasks
                .IgnoreQueryFilters() // Include deleted tasks to see the difference
                .OrderBy(t => t.Id)
                .Take(5)
                .ToListAsync(cancellationToken);

            if (!allTasks.Any())
            {
                Console.WriteLine("No tasks found in database. Cannot demonstrate soft delete.\n");
                return;
            }

            var task = allTasks.First();

            Console.WriteLine($"Working with task: ID={task.Id}, Title=\"{task.Title}\"\n");

            // 1. Show that normal query automatically filters out deleted entities
            Console.WriteLine("1. Normal Query (with Global Query Filter):");
            Console.WriteLine("   All queries automatically exclude IsDeleted = true\n");

            var activeTasks = await _context.Tasks
                .CountAsync(cancellationToken);
            Console.WriteLine($"   Active tasks (IsDeleted = false): {activeTasks}");

            var allTasksIncludingDeleted = await _context.Tasks
                .IgnoreQueryFilters() // Override global query filter
                .CountAsync(cancellationToken);
            Console.WriteLine($"   All tasks (including deleted): {allTasksIncludingDeleted}\n");

            // 2. Soft delete - set IsDeleted = true
            Console.WriteLine("2. Soft Delete Operation:");
            Console.WriteLine("   Setting IsDeleted = true (instead of physical deletion)\n");

            task.IsDeleted = true;
            task.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            Console.WriteLine($"   Task {task.Id} marked as deleted");
            Console.WriteLine($"   IsDeleted = {task.IsDeleted}");
            Console.WriteLine($"   DeletedAt = {task.DeletedAt}\n");

            // 3. Verify that deleted task is filtered out
            Console.WriteLine("3. Verify Automatic Filtering:");
            var taskAfterDelete = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken);
            
            if (taskAfterDelete == null)
            {
                Console.WriteLine($"   ✓ Task {task.Id} is automatically filtered out");
                Console.WriteLine("   ✓ Global Query Filter is working correctly\n");
            }
            else
            {
                Console.WriteLine($"   ✗ Task {task.Id} is still visible (unexpected)\n");
            }

            // 4. Use IgnoreQueryFilters() to include deleted entities
            Console.WriteLine("4. IgnoreQueryFilters() - Include Deleted Entities:");
            var deletedTask = await _context.Tasks
                .IgnoreQueryFilters() // Override global query filter
                .FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken);

            if (deletedTask != null)
            {
                Console.WriteLine($"   ✓ Found deleted task using IgnoreQueryFilters()");
                Console.WriteLine($"   Task ID: {deletedTask.Id}, IsDeleted: {deletedTask.IsDeleted}\n");
            }

            // 5. Restore - set IsDeleted = false
            Console.WriteLine("5. Restore Operation:");
            Console.WriteLine("   Setting IsDeleted = false to restore the entity\n");

            deletedTask!.IsDeleted = false;
            deletedTask.DeletedAt = null;
            await _context.SaveChangesAsync(cancellationToken);

            Console.WriteLine($"   Task {deletedTask.Id} restored");
            Console.WriteLine($"   IsDeleted = {deletedTask.IsDeleted}\n");

            // 6. Verify that restored task is visible again
            Console.WriteLine("6. Verify Restored Task is Visible:");
            var restoredTask = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken);

            if (restoredTask != null)
            {
                Console.WriteLine($"   ✓ Task {restoredTask.Id} is visible again after restore");
                Console.WriteLine($"   ✓ Global Query Filter correctly includes restored entities\n");
            }

            Console.WriteLine("=== Soft Delete Demo Completed ===\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in soft delete demonstration: {ex.Message}\n");
            Console.WriteLine($"Stack trace: {ex.StackTrace}\n");
        }
    }

    /// <summary>
    /// Demonstrates value converters
    /// Shows enum to string and JSON serialization converters
    /// </summary>
    public async Task DemonstrateValueConvertersAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Value Converters ===\n");

        try
        {
            // Get existing task or create a new one for demonstration
            var existingTask = await _context.Tasks
                .IgnoreQueryFilters()
                .OrderBy(t => t.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingTask == null)
            {
                Console.WriteLine("No tasks found. Cannot demonstrate value converters.\n");
                return;
            }

            Console.WriteLine($"Working with task: ID={existingTask.Id}, Title=\"{existingTask.Title}\"\n");

            // 1. Enum to String Converter
            Console.WriteLine("1. Enum to String Converter:");
            Console.WriteLine("   TaskPriorityLevel enum stored as string in database\n");

            existingTask.PriorityLevel = TaskPriorityLevel.High;
            await _context.SaveChangesAsync(cancellationToken);

            Console.WriteLine($"   Set PriorityLevel = {existingTask.PriorityLevel}");
            Console.WriteLine($"   In database: stored as string \"{existingTask.PriorityLevel}\" (not integer)\n");

            // Verify by reading from database
            var taskWithPriority = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == existingTask.Id, cancellationToken);

            if (taskWithPriority != null && taskWithPriority.PriorityLevel == TaskPriorityLevel.High)
            {
                Console.WriteLine($"   ✓ Value converter working: enum correctly stored and retrieved");
                Console.WriteLine($"   Retrieved PriorityLevel: {taskWithPriority.PriorityLevel}\n");
            }

            // 2. JSON Converter
            Console.WriteLine("2. JSON Converter:");
            Console.WriteLine("   TaskMetadata complex object stored as JSON string\n");

            var metadata = new TaskMetadata
            {
                Tags = "urgent,important",
                Notes = "This task requires immediate attention",
                CustomFields = new Dictionary<string, string>
                {
                    { "Department", "Engineering" },
                    { "ProjectCode", "PRJ-2024-001" }
                },
                LastReviewedAt = DateTime.UtcNow,
                ReviewNotes = "Reviewed by team lead"
            };

            existingTask.Metadata = metadata;
            await _context.SaveChangesAsync(cancellationToken);

            Console.WriteLine("   Set Metadata object:");
            Console.WriteLine($"     Tags: {metadata.Tags}");
            Console.WriteLine($"     Notes: {metadata.Notes}");
            Console.WriteLine($"     CustomFields: {string.Join(", ", metadata.CustomFields!.Select(kv => $"{kv.Key}={kv.Value}"))}");
            Console.WriteLine($"   In database: stored as JSON string\n");

            // Verify by reading from database
            var taskWithMetadata = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == existingTask.Id, cancellationToken);

            if (taskWithMetadata?.Metadata != null)
            {
                Console.WriteLine("   ✓ JSON converter working: object correctly serialized and deserialized");
                Console.WriteLine($"   Retrieved Metadata:");
                Console.WriteLine($"     Tags: {taskWithMetadata.Metadata.Tags}");
                Console.WriteLine($"     Notes: {taskWithMetadata.Metadata.Notes}");
                if (taskWithMetadata.Metadata.CustomFields != null)
                {
                    Console.WriteLine($"     CustomFields: {string.Join(", ", taskWithMetadata.Metadata.CustomFields.Select(kv => $"{kv.Key}={kv.Value}"))}");
                }
                Console.WriteLine();
            }

            // 3. Show different enum values
            Console.WriteLine("3. Different Enum Values:");
            var enumValues = Enum.GetValues<TaskPriorityLevel>();
            foreach (var enumValue in enumValues)
            {
                Console.WriteLine($"   {enumValue} = {(int)enumValue} (stored as \"{enumValue}\" in database)");
            }
            Console.WriteLine();

            Console.WriteLine("=== Value Converters Demo Completed ===\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in value converters demonstration: {ex.Message}\n");
            Console.WriteLine($"Stack trace: {ex.StackTrace}\n");
        }
    }

    /// <summary>
    /// Demonstrates owned entities (value objects)
    /// Shows how Address is stored in the same table as Project
    /// </summary>
    public async Task DemonstrateOwnedEntitiesAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Owned Entities (Value Objects) ===\n");

        try
        {
            // Get existing project or create a new one for demonstration
            var existingProject = await _context.Projects
                .IgnoreQueryFilters()
                .OrderBy(p => p.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingProject == null)
            {
                Console.WriteLine("No projects found. Cannot demonstrate owned entities.\n");
                return;
            }

            Console.WriteLine($"Working with project: ID={existingProject.Id}, Name=\"{existingProject.Name}\"\n");

            // 1. Set Address value object
            Console.WriteLine("1. Setting Address Value Object:");
            Console.WriteLine("   Address is stored in the same table as Project\n");

            existingProject.Address = new Address
            {
                Street = "123 Main Street",
                City = "Kyiv",
                ZipCode = "01001",
                Country = "Ukraine"
            };

            await _context.SaveChangesAsync(cancellationToken);

            Console.WriteLine("   Set Address:");
            Console.WriteLine($"     Street: {existingProject.Address.Street}");
            Console.WriteLine($"     City: {existingProject.Address.City}");
            Console.WriteLine($"     ZipCode: {existingProject.Address.ZipCode}");
            Console.WriteLine($"     Country: {existingProject.Address.Country}");
            Console.WriteLine("   In database: stored in same table with columns:");
            Console.WriteLine("     Address_Street, Address_City, Address_ZipCode, Address_Country\n");

            // 2. Verify by reading from database
            Console.WriteLine("2. Reading Address from Database:");
            var projectWithAddress = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == existingProject.Id, cancellationToken);

            if (projectWithAddress?.Address != null)
            {
                Console.WriteLine("   ✓ Owned entity working: Address correctly stored and retrieved");
                Console.WriteLine($"   Retrieved Address:");
                Console.WriteLine($"     Street: {projectWithAddress.Address.Street}");
                Console.WriteLine($"     City: {projectWithAddress.Address.City}");
                Console.WriteLine($"     ZipCode: {projectWithAddress.Address.ZipCode}");
                Console.WriteLine($"     Country: {projectWithAddress.Address.Country}\n");
            }

            // 3. Update Address
            Console.WriteLine("3. Updating Address:");
            existingProject.Address.City = "Lviv";
            existingProject.Address.ZipCode = "79000";
            await _context.SaveChangesAsync(cancellationToken);

            Console.WriteLine("   Updated Address:");
            Console.WriteLine($"     City: {existingProject.Address.City} (changed)");
            Console.WriteLine($"     ZipCode: {existingProject.Address.ZipCode} (changed)\n");

            // 4. Show that Address is part of Project entity
            Console.WriteLine("4. Address as Part of Project Entity:");
            Console.WriteLine("   - No separate table for Address");
            Console.WriteLine("   - Address properties stored in Projects table");
            Console.WriteLine("   - EF Core manages owned entities automatically");
            Console.WriteLine("   - Address cannot exist without Project\n");

            Console.WriteLine("=== Owned Entities Demo Completed ===\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in owned entities demonstration: {ex.Message}\n");
            Console.WriteLine($"Stack trace: {ex.StackTrace}\n");
        }
    }
}

