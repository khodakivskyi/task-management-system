using backend.CodeFirst;
using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.CodeFirst;

/// <summary>
/// Demonstration class for Code-First DbContext
/// Shows how to configure and use the new TaskManagementCodeFirstDbContext
/// </summary>
public class CodeFirstDemo
{
    private readonly TaskManagementCodeFirstDbContext _context;

    public CodeFirstDemo(TaskManagementCodeFirstDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Demonstrates that the Code-First DbContext can be created and used
    /// </summary>
    public async System.Threading.Tasks.Task RunDemonstrationAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║     Code-First DbContext Demonstration                         ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        try
        {
            // Test database connection
            Console.WriteLine("1. Testing database connection...");
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
            Console.WriteLine($"   Database connection: {(canConnect ? "✓ Connected" : "✗ Failed")}\n");

            // Show entity counts
            Console.WriteLine("2. Entity counts:");
            var userCount = await _context.Users.CountAsync(cancellationToken);
            var taskCount = await _context.Tasks.CountAsync(cancellationToken);
            var projectCount = await _context.Projects.CountAsync(cancellationToken);
            var categoryCount = await _context.Categories.CountAsync(cancellationToken);
            var statusCount = await _context.Statuses.CountAsync(cancellationToken);

            Console.WriteLine($"   Users: {userCount}");
            Console.WriteLine($"   Tasks: {taskCount}");
            Console.WriteLine($"   Projects: {projectCount}");
            Console.WriteLine($"   Categories: {categoryCount}");
            Console.WriteLine($"   Statuses: {statusCount}\n");

            // Verify relationships work
            Console.WriteLine("3. Testing relationships:");
            var taskWithRelations = await _context.Tasks
                .OrderBy(t => t.Id)
                .Include(t => t.Owner)
                .Include(t => t.Status)
                .Include(t => t.Category)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(cancellationToken);

            if (taskWithRelations != null)
            {
                Console.WriteLine($"   Task: {taskWithRelations.Title}");
                Console.WriteLine($"   Owner: {taskWithRelations.Owner?.Name}");
                Console.WriteLine($"   Status: {taskWithRelations.Status?.Name}");
                Console.WriteLine($"   Category: {taskWithRelations.Category?.Name ?? "None"}");
                Console.WriteLine($"   Project: {taskWithRelations.Project?.Name ?? "None"}\n");
            }
            else
            {
                Console.WriteLine("   No tasks found in database\n");
            }

            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║     Code-First DbContext Demonstration Completed              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Code-First demonstration: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}

