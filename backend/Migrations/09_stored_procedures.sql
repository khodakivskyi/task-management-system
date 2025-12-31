-- get project statistics
drop function if exists get_project_statistics(int);

create or replace function get_project_statistics(project_id_param int)
returns table (
    total_tasks int,
    completed_tasks int,
    in_progress_tasks int,
    overdue_tasks int,
    total_estimated_hours int,
    total_actual_hours int
) as $$
begin
    return query
    select 
        count(*)::int as total_tasks,
        count(*) filter (where s."Name" = 'Done')::int as completed_tasks,
        count(*) filter (where s."Name" = 'In Progress')::int as in_progress_tasks,
        count(*) filter (where t."Deadline" < now() and s."Name" != 'Done')::int as overdue_tasks,
        coalesce(sum(t."EstimatedHours"), 0)::int as total_estimated_hours,
        coalesce(sum(t."ActualHours"), 0)::int as total_actual_hours
    from "Tasks" t
    inner join "Statuses" s on t."StatusId" = s."Id"
    where t."ProjectId" = project_id_param;
end;
$$ language plpgsql;



-- search tasks with filters
drop function if exists search_tasks(int, int, int, int, int, varchar);

create or replace function search_tasks(
    user_id_param int default null,
    project_id_param int default null,
    status_id_param int default null,
    priority_min int default null,
    priority_max int default null,
    search_text varchar default null
)
returns table (
    id int,
    title varchar,
    status_name varchar,
    priority int,
    deadline timestamp
) as $$
begin
    return query
    select 
        t."Id",
        t."Title",
        s."Name" as status_name,
        t."Priority",
        t."Deadline"
    from "Tasks" t
    inner join "Statuses" s on t."StatusId" = s."Id"
    where 
        (user_id_param is null or t."OwnerId" = user_id_param)
        and (project_id_param is null or t."ProjectId" = project_id_param)
        and (status_id_param is null or t."StatusId" = status_id_param)
        and (priority_min is null or t."Priority" >= priority_min)
        and (priority_max is null or t."Priority" <= priority_max)
        and (search_text is null or t."Title" ilike '%' || search_text || '%' or t."Description" ilike '%' || search_text || '%')
    order by t."CreatedAt" desc;
end;
$$ language plpgsql;



-- calculate project progress
drop function if exists calculate_project_progress(int);

create or replace function calculate_project_progress(project_id_param int)
returns decimal(5,2) as $$
declare
    total_tasks int;
    completed_tasks int;
    progress decimal(5,2);
begin
    select count(*) into total_tasks
    from "Tasks"
    where "ProjectId" = project_id_param;
    
    if total_tasks = 0 then
        return 0.00;
    end if;
    
    select count(*) into completed_tasks
    from "Tasks" t
    inner join "Statuses" s on t."StatusId" = s."Id"
    where t."ProjectId" = project_id_param and s."Name" = 'Done';
    
    progress := (completed_tasks:: decimal / total_tasks::decimal) * 100;
    
    return round(progress, 2);
end;
$$ language plpgsql;



-- archive old completed tasks
drop function if exists archive_old_completed_tasks();

create or replace function archive_old_completed_tasks()
returns int as $$
declare
    archived_count int;
begin
    -- create archive table if not exists
    create table if not exists "TasksArchive" (like "Tasks" including all);
    
    -- move to archive table (insert only if not already archived)
    insert into "TasksArchive"
    select t.*
    from "Tasks" t
    inner join "Statuses" s on t."StatusId" = s."Id"
    where s."Name" = 'Done' 
      and t."UpdatedAt" < now() - interval '6 months'
      and not exists (
          select 1 from "TasksArchive" ta where ta."Id" = t."Id"
      );
    
    get diagnostics archived_count = row_count;
    
    -- delete from main table
    delete from "Tasks" t
    using "Statuses" s
    where t."StatusId" = s."Id"
      and s."Name" = 'Done'
      and t."UpdatedAt" < now() - interval '6 months';
    
    return archived_count;
end;
$$ language plpgsql;
