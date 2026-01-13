-- Fix project statistics function to return PascalCase column names for C# mapping

drop function if exists get_project_statistics(int);

create or replace function get_project_statistics(project_id_param int)
returns table (
    "TotalTasks" int,
    "CompletedTasks" int,
    "InProgressTasks" int,
    "OverdueTasks" int,
    "TotalEstimatedHours" int,
    "TotalActualHours" int
) as $$
begin
    return query
    select 
        count(*)::int as "TotalTasks",
        count(*) filter (where s."Name" = 'Done')::int as "CompletedTasks",
        count(*) filter (where s."Name" = 'In Progress')::int as "InProgressTasks",
        count(*) filter (where t."Deadline" < now() and s."Name" != 'Done')::int as "OverdueTasks",
        coalesce(sum(t."EstimatedHours"), 0)::int as "TotalEstimatedHours",
        coalesce(sum(t."ActualHours"), 0)::int as "TotalActualHours"
    from "Tasks" t
    left join "Statuses" s on t."StatusId" = s."Id"
    where t."ProjectId" = project_id_param;
end;
$$ language plpgsql;
