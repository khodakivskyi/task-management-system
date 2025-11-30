-- ============================================
-- Indexes for Users table
-- ============================================

-- Unique index for login (for fast authentication)
create unique index "IX_Users_Login" on "Users"("Login");

-- Indexes for searching users by name and surname
create index "IX_Users_Name" on "Users"("Name");
create index "IX_Users_Surname" on "Users"("Surname");

-- Composite index for searching by name and surname
create index "IX_Users_Name_Surname" on "Users"("Name", "Surname");

-- ============================================
-- Indexes for Projects table
-- ============================================

-- Index for searching projects by owner
create index "IX_Projects_OwnerId" on "Projects"("OwnerId");

-- Index for searching projects by name
create index "IX_Projects_Name" on "Projects"("Name");

-- Indexes for filtering by dates
create index "IX_Projects_StartDate" on "Projects"("StartDate");
create index "IX_Projects_EndDate" on "Projects"("EndDate");

-- ============================================
-- Indexes for Tasks table
-- ============================================

-- Indexes for foreign keys (for fast JOIN operations)
create index "IX_Tasks_OwnerId" on "Tasks"("OwnerId");
create index "IX_Tasks_StatusId" on "Tasks"("StatusId");
create index "IX_Tasks_CategoryId" on "Tasks"("CategoryId");
create index "IX_Tasks_ProjectId" on "Tasks"("ProjectId");

-- Index for searching tasks by deadline (for reminders and filtering)
create index "IX_Tasks_Deadline" on "Tasks"("Deadline");

-- Index for sorting by priority
create index "IX_Tasks_Priority" on "Tasks"("Priority");

-- Indexes for sorting by dates
create index "IX_Tasks_CreatedAt" on "Tasks"("CreatedAt");
create index "IX_Tasks_UpdatedAt" on "Tasks"("UpdatedAt");

-- Composite index for searching project tasks by status (often used together)
create index "IX_Tasks_ProjectId_StatusId" on "Tasks"("ProjectId", "StatusId");

-- Composite index for searching user tasks by status
create index "IX_Tasks_OwnerId_StatusId" on "Tasks"("OwnerId", "StatusId");

-- Composite index for searching tasks with deadline in project
create index "IX_Tasks_ProjectId_Deadline" on "Tasks"("ProjectId", "Deadline") where "Deadline" is not null;

-- ============================================
-- Indexes for TaskAssignees table
-- ============================================

-- Indexes for foreign keys
create index "IX_TaskAssignees_TaskId" on "TaskAssignees"("TaskId");
create index "IX_TaskAssignees_UserId" on "TaskAssignees"("UserId");

-- Composite unique index to check if user is already assigned to task
create unique index "IX_TaskAssignees_TaskId_UserId" on "TaskAssignees"("TaskId", "UserId");

-- Composite index for searching all user tasks
create index "IX_TaskAssignees_UserId_TaskId" on "TaskAssignees"("UserId", "TaskId");

-- ============================================
-- Indexes for ProjectMembers table
-- ============================================

-- Indexes for foreign keys
create index "IX_ProjectMembers_ProjectId" on "ProjectMembers"("ProjectId");
create index "IX_ProjectMembers_UserId" on "ProjectMembers"("UserId");
create index "IX_ProjectMembers_RoleId" on "ProjectMembers"("RoleId");

-- Composite unique index to check if user is already a project member
create unique index "IX_ProjectMembers_ProjectId_UserId" on "ProjectMembers"("ProjectId", "UserId");

-- Composite index for searching all user projects
create index "IX_ProjectMembers_UserId_ProjectId" on "ProjectMembers"("UserId", "ProjectId");

-- Index for searching project members by role
create index "IX_ProjectMembers_ProjectId_RoleId" on "ProjectMembers"("ProjectId", "RoleId");

-- ============================================
-- Indexes for Comments table
-- ============================================

-- Indexes for foreign keys
create index "IX_Comments_TaskId" on "Comments"("TaskId");
create index "IX_Comments_UserId" on "Comments"("UserId");

-- Index for sorting comments by creation date
create index "IX_Comments_CreatedAt" on "Comments"("CreatedAt");

-- Composite index for searching task comments with sorting
create index "IX_Comments_TaskId_CreatedAt" on "Comments"("TaskId", "CreatedAt");

-- ============================================
-- Indexes for TaskHistory table
-- ============================================

-- Indexes for foreign keys
create index "IX_TaskHistory_TaskId" on "TaskHistory"("TaskId");
create index "IX_TaskHistory_UserId" on "TaskHistory"("UserId");

-- Index for sorting by change date
create index "IX_TaskHistory_ChangedAt" on "TaskHistory"("ChangedAt");

-- Composite index for searching task history with chronological sorting
create index "IX_TaskHistory_TaskId_ChangedAt" on "TaskHistory"("TaskId", "ChangedAt");

-- Index for searching changes by field
create index "IX_TaskHistory_FieldName" on "TaskHistory"("FieldName");

-- Composite index for searching user changes in task
create index "IX_TaskHistory_TaskId_UserId" on "TaskHistory"("TaskId", "UserId");

-- ============================================
-- Indexes for Favorites table
-- ============================================

-- Index for foreign key
create index "IX_Favorites_UserId" on "Favorites"("UserId");

-- Composite unique index to check if object is already in favorites
create unique index "IX_Favorites_UserId_EntityType_EntityId" on "Favorites"("UserId", "EntityType", "EntityId");

-- Index for searching favorites by entity type
create index "IX_Favorites_EntityType_EntityId" on "Favorites"("EntityType", "EntityId");

-- Index for sorting by addition date
create index "IX_Favorites_CreatedAt" on "Favorites"("CreatedAt");

-- Composite index for searching user favorites by type
create index "IX_Favorites_UserId_EntityType" on "Favorites"("UserId", "EntityType");

-- ============================================
-- Additional indexes for optimization
-- ============================================

-- Index for searching tasks without project (standalone tasks)
create index "IX_Tasks_ProjectId_Null" on "Tasks"("ProjectId") where "ProjectId" is null;