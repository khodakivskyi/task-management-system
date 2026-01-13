-- Fix project statistics function to use left join instead of inner join

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
    left join "Statuses" s on t."StatusId" = s."Id"
    where t."ProjectId" = project_id_param;
end;
$$ language plpgsql;
