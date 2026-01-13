-- Fix search_tasks functionto return PascalCase column names for C# mapping

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
    "Id" int,
    "Title" varchar,
    "StatusName" varchar,
    "Priority" int,
    "Deadline" timestamptz
) as $$
begin
    return query
    select 
        t."Id",
        t."Title",
        s."Name" as "StatusName",
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
