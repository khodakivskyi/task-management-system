-- ============================================
-- Seed Test Data for Complete Database
-- ============================================

-- Insert test users (only if they don't exist)
insert into "Users" ("Name", "Surname", "Login", "PasswordHash", "Salt", "CreatedAt")
select * from (values
    ('John', 'Doe', 'john.doe', 'hash1', 'salt1', current_timestamp),
    ('Jane', 'Smith', 'jane.smith', 'hash2', 'salt2', current_timestamp),
    ('Bob', 'Johnson', 'bob.johnson', 'hash3', 'salt3', current_timestamp),
    ('Alice', 'Williams', 'alice.williams', 'hash4', 'salt4', current_timestamp),
    ('Charlie', 'Brown', 'charlie.brown', 'hash5', 'salt5', current_timestamp)
) as v("Name", "Surname", "Login", "PasswordHash", "Salt", "CreatedAt")
where not exists (select 1 from "Users" where "Login" = v."Login");

-- Insert test statuses (only if they don't exist)
insert into "Statuses" ("Name", "Color")
select * from (values
    ('To Do', '#FF5733'),
    ('In Progress', '#FFC300'),
    ('Review', '#3498DB'),
    ('Done', '#2ECC71'),
    ('Blocked', '#E74C3C')
) as v("Name", "Color")
where not exists (select 1 from "Statuses" where "Name" = v."Name");

-- Insert test categories (only if they don't exist)
insert into "Categories" ("Name", "Color")
select * from (values
    ('Development', '#3498DB'),
    ('Design', '#9B59B6'),
    ('Testing', '#F39C12'),
    ('Documentation', '#1ABC9C'),
    ('Bug Fix', '#E74C3C')
) as v("Name", "Color")
where not exists (select 1 from "Categories" where "Name" = v."Name");

-- Insert test projects (using existing users as owners, only if they don't exist)
insert into "Projects" ("OwnerId", "Name", "Description", "StartDate", "EndDate")
select
    u."Id",
    'Project ' || u."Id",
    'Test project description ' || u."Id",
    current_timestamp - interval '30 days',
    current_timestamp + interval '60 days'
from "Users" u
where not exists (
    select 1 from "Projects" p 
    where p."OwnerId" = u."Id" and p."Name" = 'Project ' || u."Id"
)
limit 5;

-- Insert project roles (only if they don't exist)
insert into "ProjectRoles" ("Name", "CanCreateTasks", "CanEditTasks", "CanDeleteTasks", "CanAssignTasks", "CanManageMembers")
select * from (values
    ('Admin', true, true, true, true, true),
    ('Developer', true, true, false, true, false),
    ('Tester', true, false, false, false, false),
    ('Viewer', false, false, false, false, false)
) as v("Name", "CanCreateTasks", "CanEditTasks", "CanDeleteTasks", "CanAssignTasks", "CanManageMembers")
where not exists (select 1 from "ProjectRoles" where "Name" = v."Name");

-- Insert project members (assign users to projects with roles)
insert into "ProjectMembers" ("ProjectId", "UserId", "RoleId", "JoinedAt")
select 
    p."Id",
    u."Id",
    pr."Id",
    current_timestamp - interval '10 days'
from "Projects" p
cross join "Users" u
cross join "ProjectRoles" pr
where pr."Name" = case 
    when u."Login" like 'john%' then 'Admin'
    when u."Login" like 'jane%' then 'Developer'
    when u."Login" like 'bob%' then 'Developer'
    when u."Login" like 'alice%' then 'Tester'
    else 'Viewer'
end
and not exists (
    select 1 from "ProjectMembers" pm 
    where pm."ProjectId" = p."Id" and pm."UserId" = u."Id"
)
limit 20;

-- Insert test tasks
insert into "Tasks" (
    "OwnerId", "StatusId", "CategoryId", "ProjectId",
    "Title", "Description", "Priority", "Deadline",
    "CreatedAt", "EstimatedHours", "ActualHours"
)
select
    u."Id",
    s."Id",
    c."Id",
    p."Id",
    'Task ' || row_number() over (order by u."Id", s."Id", c."Id"),
    'Description for task ' || row_number() over (order by u."Id", s."Id", c."Id"),
    (random() * 4 + 1)::integer,
    current_timestamp + (random() * 30)::integer * interval '1 day',
    current_timestamp - (random() * 30)::integer * interval '1 day',
    (random() * 39 + 1)::integer,
    (random() * 20)::integer
from "Users" u
cross join "Statuses" s
cross join "Categories" c
cross join "Projects" p
where not exists (
    select 1 from "Tasks" t 
    where t."OwnerId" = u."Id" 
    and t."StatusId" = s."Id" 
    and t."CategoryId" = c."Id"
    and t."ProjectId" = p."Id"
)
limit 50;

-- Insert task assignees (assign users to tasks)
insert into "TaskAssignees" ("TaskId", "UserId")
select
    t."Id",
    u."Id"
from "Tasks" t
cross join "Users" u
where u."Id" != t."OwnerId"
and random() < 0.3  -- Assign 30% of users to tasks
and not exists (
    select 1 from "TaskAssignees" ta 
    where ta."TaskId" = t."Id" and ta."UserId" = u."Id"
)
limit 30;

-- Insert comments on tasks
insert into "Comments" ("TaskId", "UserId", "Content", "CreatedAt")
select
    t."Id",
    u."Id",
    'Comment on task ' || t."Title" || ' by ' || u."Name",
    t."CreatedAt" + (random() * 10)::integer * interval '1 day'
from "Tasks" t
cross join "Users" u
where random() < 0.4  -- 40% of tasks have comments
and not exists (
    select 1 from "Comments" c 
    where c."TaskId" = t."Id" and c."UserId" = u."Id"
)
limit 25;

-- Insert task history (track changes to tasks)
insert into "TaskHistory" ("TaskId", "UserId", "FieldName", "OldValue", "NewValue", "ChangedAt")
select
    t."Id",
    t."OwnerId",
    case (random() * 3)::integer
        when 0 then 'StatusId'
        when 1 then 'Priority'
        else 'Description'
    end,
    case (random() * 3)::integer
        when 0 then 'Old Status'
        when 1 then '3'
        else 'Old description'
    end,
    case (random() * 3)::integer
        when 0 then 'New Status'
        when 1 then '5'
        else 'New description'
    end,
    t."UpdatedAt" - (random() * 5)::integer * interval '1 day'
from "Tasks" t
where random() < 0.5  -- 50% of tasks have history
and not exists (
    select 1 from "TaskHistory" th 
    where th."TaskId" = t."Id"
)
limit 20;

-- Insert favorites (users favorite tasks and projects)
insert into "Favorites" ("UserId", "EntityTypeId", "EntityId", "CreatedAt")
select
    u."Id",
    et."Id",
    t."Id",
    current_timestamp - (random() * 20)::integer * interval '1 day'
from "Users" u
cross join "Tasks" t
cross join "EntityTypes" et
where et."Name" = 'task'
and random() < 0.2
and not exists (
    select 1 from "Favorites" f 
    where f."UserId" = u."Id" 
    and f."EntityTypeId" = et."Id"
    and f."EntityId" = t."Id"
)
limit 15;

insert into "Favorites" ("UserId", "EntityTypeId", "EntityId", "CreatedAt")
select
    u."Id",
    et."Id",
    p."Id",
    current_timestamp - (random() * 20)::integer * interval '1 day'
from "Users" u
cross join "Projects" p
cross join "EntityTypes" et
where et."Name" = 'project'
and random() < 0.3
and not exists (
    select 1 from "Favorites" f 
    where f."UserId" = u."Id" 
    and f."EntityTypeId" = et."Id"
    and f."EntityId" = p."Id"
)
limit 10;

