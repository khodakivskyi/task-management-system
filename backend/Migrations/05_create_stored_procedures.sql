-- ============================================
-- Stored Procedures for Task Management
-- ============================================

-- Stored Procedure for creating a task with validation and business logic
-- Uses INPUT parameters for data, OUTPUT parameters for results, and exception handling
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
    
    -- Business logic: Create the task
    insert into "Tasks" (
        "OwnerId", "StatusId", "CategoryId", "ProjectId",
        "Title", "Description", "Priority", "Deadline",
        "CreatedAt", "UpdatedAt", "EstimatedHours", "ActualHours"
    )
    values (
        p_owner_id, p_status_id, p_category_id, p_project_id,
        trim(p_title), p_description, p_priority, p_deadline,
        current_timestamp, current_timestamp, p_estimated_hours, p_actual_hours
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

