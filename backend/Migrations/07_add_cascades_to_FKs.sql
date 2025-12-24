-- Tasks: deleting a task removes all dependent records
do $$
begin
  if not exists (
    select 1 from pg_constraint where conname = 'FK_TaskAssignees_Task'
  ) then
    alter table "TaskAssignees"
      add constraint "FK_TaskAssignees_Task"
      foreign key ("TaskId") references "Tasks"("Id") on delete cascade;
  end if;
end$$;

do $$
begin
  if not exists (
    select 1 from pg_constraint where conname = 'FK_Comments_Task'
  ) then
    alter table "Comments"
      add constraint "FK_Comments_Task"
      foreign key ("TaskId") references "Tasks"("Id") on delete cascade;
  end if;
end$$;

do $$
begin
  if not exists (
    select 1 from pg_constraint where conname = 'FK_TaskHistory_Task'
  ) then
    alter table "TaskHistory"
      add constraint "FK_TaskHistory_Task"
      foreign key ("TaskId") references "Tasks"("Id") on delete cascade;
  end if;
end$$;

-- Projects: deleting a project removes all tasks and their dependencies
do $$
begin
  if not exists (
    select 1 from pg_constraint where conname = 'FK_Tasks_Project'
  ) then
    alter table "Tasks"
      add constraint "FK_Tasks_Project"
      foreign key ("ProjectId") references "Projects"("Id") on delete cascade;
  end if;
end$$;

-- ProjectMembers: deleting a project removes all project members
do $$
begin
  if not exists (
    select 1 from pg_constraint where conname = 'FK_ProjectMembers_Project'
  ) then
    alter table "ProjectMembers"
      add constraint "FK_ProjectMembers_Project"
      foreign key ("ProjectId") references "Projects"("Id") on delete cascade;
  end if;
end$$;

-- Users: deleting a user removes all their favorites
do $$
begin
  if not exists (
    select 1 from pg_constraint where conname = 'FK_Favorites_User'
  ) then
    alter table "Favorites"
      add constraint "FK_Favorites_User"
      foreign key ("UserId") references "Users"("Id") on delete cascade;
  end if;
end$$;

-- ProjectRoles: deleting a role removes all assignments of that role
do $$
begin
  if not exists (
    select 1 from pg_constraint where conname = 'FK_ProjectMembers_Role'
  ) then
    alter table "ProjectMembers"
      add constraint "FK_ProjectMembers_Role"
      foreign key ("RoleId") references "ProjectRoles"("Id") on delete cascade;
  end if;
end$$;
