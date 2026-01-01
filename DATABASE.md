# Database Documentation

## Schema Overview

### Tables
- **Users** - User accounts
- **Projects** - Project management
- **Tasks** - Task items
- **Statuses** - Task statuses (To Do, In Progress, etc.)
- **Categories** - Task categories
- **ProjectRoles** - Role definitions for project members
- **ProjectMembers** - User-project relationships
- **TaskAssignees** - Task-user assignments
- **Comments** - Task comments
- **TaskHistory** - Task change history
- **Favorites** - User favorites (tasks/projects)
- **EntityTypes** - Reference table for entity types

### Key Relationships
- Tasks → Projects (optional)
- Tasks → Users (OwnerId)
- Tasks → Statuses, Categories
- Projects → Users (OwnerId)
- ProjectMembers → Projects, Users, ProjectRoles
- Comments, TaskHistory → Tasks, Users
- Favorites → Users, EntityTypes

---

## Constraints

### Check Constraints
- **Tasks.Priority:** 1-5
- **Tasks.EstimatedHours:** >= 0
- **Tasks.ActualHours:** >= 0
- **Tasks.UpdatedAt:** >= CreatedAt
- **Projects.EndDate:** >= StartDate
- **Statuses.Color:** Hex format (#RRGGBB)
- **Categories.Color:** Hex format (#RRGGBB)
- **Comments.Content:** Not empty after trim
- **Favorites.EntityType:** 'task' or 'project' (legacy, now uses EntityTypes table)

### Foreign Keys
All foreign keys have CASCADE DELETE where appropriate:
- Tasks → Projects (CASCADE)
- Tasks → Users, Statuses, Categories
- Comments → Tasks (CASCADE), Users
- TaskHistory → Tasks (CASCADE), Users
- TaskAssignees → Tasks (CASCADE), Users
- ProjectMembers → Projects (CASCADE), Users, ProjectRoles (CASCADE)
- Favorites → Users (CASCADE), EntityTypes

---

## Indexes

### Performance Indexes
- Unique indexes on login fields
- Foreign key indexes for JOIN optimization
- Composite indexes for common query patterns
- Partial indexes for filtered queries (e.g., `WHERE ProjectId IS NULL`)

**Key composite indexes:**
- `IX_Tasks_ProjectId_StatusId` - Filter tasks by project and status
- `IX_Tasks_OwnerId_StatusId` - Filter user tasks by status
- `IX_Comments_TaskId_CreatedAt` - Sort task comments
- `IX_TaskHistory_TaskId_ChangedAt` - Chronological task history
- `IX_ProjectMembers_ProjectId_UserId` - Unique membership check

---

## Stored Procedures

#### `create_task_with_validation(p_owner_id, p_status_id, p_title, ...)`
Creates a task with validation (checks owner/status/category/project existence, priority 1-5, non-empty title). Returns `p_task_id`, `p_created_at`, `p_message`.

```sql
SELECT * FROM create_task_with_validation(1, 2, 'New Task', 1, 1, 'Description', 3, '2024-12-31'::timestamp, 8, 0);
```

#### `get_project_statistics(project_id_param)`
Returns project statistics: total_tasks, completed_tasks, in_progress_tasks, overdue_tasks, total_estimated_hours, total_actual_hours.

```sql
SELECT * FROM get_project_statistics(1);
```

#### `calculate_project_progress(project_id_param)`
Returns project completion percentage (0.00-100.00) based on completed tasks.

```sql
SELECT calculate_project_progress(1);
```

#### `search_tasks(user_id_param, project_id_param, status_id_param, priority_min, priority_max, search_text)`
Searches tasks with filters (all parameters optional). Returns id, title, status_name, priority, deadline.

```sql
SELECT * FROM search_tasks(user_id_param := 1, status_id_param := 2, search_text := 'important');
```

#### `archive_old_completed_tasks()`
Archives completed tasks older than 6 months to `TasksArchive`. Returns count of archived tasks.

```sql
SELECT archive_old_completed_tasks();
```

---

## Triggers

#### `trigger_tasks_updated_at` (BEFORE UPDATE on `Tasks`)
Automatically updates `UpdatedAt = NOW()` when a task is modified.

#### `trigger_log_task_changes` (AFTER UPDATE on `Tasks`)
Automatically logs all task field changes to `TaskHistory` (Title, StatusId, Priority, Description, CategoryId, ProjectId, Deadline, EstimatedHours, ActualHours, OwnerId).

#### `trigger_update_project_task_count` (AFTER INSERT/UPDATE/DELETE on `Tasks`)
Automatically maintains `Projects.TaskCount` - increments/decrements counter when tasks are added/deleted or ProjectId changes.

---

## Migration System

Migrations are tracked in `__MigrationsHistory` table with:
- Version number
- File name
- SHA256 checksum
- Execution timestamp
- Execution time

**Migration files:**
1. `01_create_tables.sql` - Base schema
2. `02_add_indexes.sql` - Performance indexes
3. `03_add_check_constraints.sql` - Data validation
4. `04_create_entity_types_and_update_favorites.sql` - Entity types refactor
5. `05_create_stored_procedures.sql` - Stored procedures (task creation)
6. `06_seed_test_data.sql` - Test data
7. `07_add_cascades_to_FKs.sql` - Cascade deletes
8. `08_add_triggers.sql` - Triggers and validation
9. `09_stored_procedures.sql` - Additional stored procedures (statistics, search, archive)

---

## Useful Queries

### Get task with details
```sql
SELECT t.*, s."Name" as StatusName, c."Name" as CategoryName, p."Name" as ProjectName
FROM "Tasks" t
LEFT JOIN "Statuses" s ON t."StatusId" = s."Id"
LEFT JOIN "Categories" c ON t."CategoryId" = c."Id"
LEFT JOIN "Projects" p ON t."ProjectId" = p."Id"
WHERE t."Id" = 1;
```

### Get task history
```sql
SELECT * FROM "TaskHistory"
WHERE "TaskId" = 1
ORDER BY "ChangedAt" DESC;
```

### Get project task count
```sql
SELECT "Id", "Name", "TaskCount" FROM "Projects";
```

### Get user's tasks by status
```sql
SELECT * FROM "Tasks"
WHERE "OwnerId" = 1 AND "StatusId" = 2
ORDER BY "Priority" DESC, "Deadline" ASC;
```
