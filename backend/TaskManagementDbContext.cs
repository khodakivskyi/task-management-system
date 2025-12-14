using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using backend.EFModels;

namespace backend;

public partial class TaskManagementDbContext : DbContext
{
    public TaskManagementDbContext()
    {
    }

    public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<EntityType> EntityTypes { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<MigrationsHistory> MigrationsHistories { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectMember> ProjectMembers { get; set; }

    public virtual DbSet<ProjectRole> ProjectRoles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<TaskModel> Tasks { get; set; }

    public virtual DbSet<TaskAssignee> TaskAssignees { get; set; }

    public virtual DbSet<TaskHistory> TaskHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Categories_pkey");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Comments_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Task).WithMany(p => p.Comments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Task");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_User");
        });

        modelBuilder.Entity<EntityType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("EntityTypes_pkey");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Favorites_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.EntityType).WithMany(p => p.Favorites)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Favorites_EntityType");

            entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Favorites_User");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Projects_pkey");

            entity.HasOne(d => d.Owner).WithMany(p => p.Projects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projects_Owner");
        });

        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ProjectMembers_pkey");

            entity.Property(e => e.JoinedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectMembers_Project");

            entity.HasOne(d => d.Role).WithMany(p => p.ProjectMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectMembers_Role");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectMembers_User");
        });

        modelBuilder.Entity<ProjectRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ProjectRoles_pkey");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Statuses_pkey");
        });

        modelBuilder.Entity<TaskModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Tasks_pkey");

            entity.HasIndex(e => new { e.ProjectId, e.Deadline }, "IX_Tasks_ProjectId_Deadline").HasFilter("(\"Deadline\" IS NOT NULL)");

            entity.HasIndex(e => e.ProjectId, "IX_Tasks_ProjectId_Null").HasFilter("(\"ProjectId\" IS NULL)");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Category).WithMany(p => p.Tasks).HasConstraintName("FK_Tasks_Category");

            entity.HasOne(d => d.Owner).WithMany(p => p.Tasks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tasks_Owner");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks).HasConstraintName("FK_Tasks_Project");

            entity.HasOne(d => d.Status).WithMany(p => p.Tasks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tasks_Status");
        });

        modelBuilder.Entity<TaskAssignee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TaskAssignees_pkey");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskAssignees)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskAssignees_Task");

            entity.HasOne(d => d.User).WithMany(p => p.TaskAssignees)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskAssignees_User");
        });

        modelBuilder.Entity<TaskHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TaskHistory_pkey");

            entity.Property(e => e.ChangedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskHistory_Task");

            entity.HasOne(d => d.User).WithMany(p => p.TaskHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskHistory_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
