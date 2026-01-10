-- ============================================
-- Migration: Convert timestamp to timestamptz
-- Purpose: Add timezone support to all timestamp columns
-- ============================================

-- convert Tasks table timestamp columns to timestamptz
alter table "Tasks" 
  alter column "Deadline" type timestamptz using "Deadline" at time zone 'UTC',
  alter column "CreatedAt" type timestamptz using "CreatedAt" at time zone 'UTC',
  alter column "UpdatedAt" type timestamptz using "UpdatedAt" at time zone 'UTC';

-- convert Users table timestamp columns to timestamptz
alter table "Users"
  alter column "CreatedAt" type timestamptz using "CreatedAt" at time zone 'UTC';

-- add LastLoginAt column if it doesn't exist and convert to timestamptz
do $$ 
begin
  if not exists (
    select 1 from information_schema.columns 
    where table_name = 'Users' and column_name = 'LastLoginAt'
  ) then
    alter table "Users" add column "LastLoginAt" timestamptz;
  else
    alter table "Users" 
      alter column "LastLoginAt" type timestamptz using "LastLoginAt" at time zone 'UTC';
  end if;
end $$;

-- convert Projects table timestamp columns to timestamptz
alter table "Projects"
  alter column "StartDate" type timestamptz using "StartDate" at time zone 'UTC',
  alter column "EndDate" type timestamptz using "EndDate" at time zone 'UTC';

-- convert Comments table timestamp columns to timestamptz
alter table "Comments"
  alter column "CreatedAt" type timestamptz using "CreatedAt" at time zone 'UTC';

-- convert TaskHistory table timestamp columns to timestamptz
alter table "TaskHistory"
  alter column "ChangedAt" type timestamptz using "ChangedAt" at time zone 'UTC';

-- convert ProjectMembers table timestamp columns to timestamptz
alter table "ProjectMembers"
  alter column "JoinedAt" type timestamptz using "JoinedAt" at time zone 'UTC';

-- convert Favorites table timestamp columns to timestamptz
alter table "Favorites"
  alter column "CreatedAt" type timestamptz using "CreatedAt" at time zone 'UTC';

-- update default values to use timezone('UTC', now()) for all timestamp columns
alter table "Tasks" 
  alter column "CreatedAt" set default timezone('UTC', now()),
  alter column "UpdatedAt" set default timezone('UTC', now());

alter table "Users"
  alter column "CreatedAt" set default timezone('UTC', now());

alter table "Comments"
  alter column "CreatedAt" set default timezone('UTC', now());

alter table "TaskHistory"
  alter column "ChangedAt" set default timezone('UTC', now());

alter table "ProjectMembers"
  alter column "JoinedAt" set default timezone('UTC', now());

alter table "Favorites"
  alter column "CreatedAt" set default timezone('UTC', now());

-- ============================================
-- Update stored procedure: create_task_with_validation
-- Replace current_timestamp with timezone('UTC', now())
-- ============================================

create or replace function create_task_with_validation(
    -- INPUT parameters
    p_owner_id integer,
    p_status_id integer,
    p_title varchar(50),
    p_category_id integer default null,
    p_project_id integer default null,
    p_description varchar(250) default null,
    p_priority integer default null,
    p_deadline timestamp default null,
    p_estimated_hours integer default 0,
    p_actual_hours integer default 0,
    
    -- OUTPUT parameters
    out p_task_id integer,
    out p_created_at timestamp,
    out p_message varchar(255)
)
returns record
language plpgsql
as $$
declare
    v_user_exists boolean;
    v_status_exists boolean;
    v_category_exists boolean;
    v_project_exists boolean;
begin
    -- Initialize OUTPUT parameters
    p_task_id := null;
    p_created_at := null;
    p_message := '';
    
    -- Validation: Check if Owner exists
    select exists(select 1 from "Users" where "Id" = p_owner_id) into v_user_exists;
    if not v_user_exists then
        p_message := 'User with Id ' || p_owner_id || ' does not exist';
        return;
    end if;
    
    -- Validation: Check if Status exists
    select exists(select 1 from "Statuses" where "Id" = p_status_id) into v_status_exists;
    if not v_status_exists then
        p_message := 'Status with Id ' || p_status_id || ' does not exist';
        return;
    end if;
    
    -- Validation: Check if Category exists (if provided)
    if p_category_id is not null then
        select exists(select 1 from "Categories" where "Id" = p_category_id) into v_category_exists;
        if not v_category_exists then
            p_message := 'Category with Id ' || p_category_id || ' does not exist';
            return;
        end if;
    end if;
    
    -- Validation: Check if Project exists (if provided)
    if p_project_id is not null then
        select exists(select 1 from "Projects" where "Id" = p_project_id) into v_project_exists;
        if not v_project_exists then
            p_message := 'Project with Id ' || p_project_id || ' does not exist';
            return;
        end if;
    end if;
    
    -- Validation: Title cannot be empty
    if p_title is null or length(trim(p_title)) = 0 then
        p_message := 'Title cannot be empty';
        return;
    end if;
    
    -- Validation: Priority must be between 1 and 5
    if p_priority is not null and (p_priority < 1 or p_priority > 5) then
        p_message := 'Priority must be between 1 and 5';
        return;
    end if;
    
    -- Business logic: Create the task (using UTC timezone)
    insert into "Tasks" (
        "OwnerId", "StatusId", "CategoryId", "ProjectId",
        "Title", "Description", "Priority", "Deadline",
        "CreatedAt", "UpdatedAt", "EstimatedHours", "ActualHours"
    )
    values (
        p_owner_id, p_status_id, p_category_id, p_project_id,
        trim(p_title), p_description, p_priority, p_deadline,
        timezone('UTC', now()), timezone('UTC', now()), p_estimated_hours, p_actual_hours
    )
    returning "Id", "CreatedAt" into p_task_id, p_created_at;
    
    -- Success message
    p_message := 'Task created successfully';
    
exception
    when others then
        p_message := 'Error: ' || sqlerrm;
        raise;
end;
$$;
