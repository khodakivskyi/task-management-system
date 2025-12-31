-- automatic updatedat update
drop trigger if exists trigger_tasks_updated_at on "Tasks";
drop function if exists update_updated_at_column();

create or replace function update_updated_at_column()
returns trigger as $$
begin
    NEW."UpdatedAt" = NOW();
    return NEW;
end;
$$ language plpgsql;

create trigger trigger_tasks_updated_at
    before update on "Tasks"
    for each row
    execute function update_updated_at_column();



-- automatic change logging (taskhistory)
drop trigger if exists trigger_log_task_changes on "Tasks";
drop function if exists log_task_changes();

create or replace function log_task_changes()
returns trigger as $$
begin
    -- log title change
    if OLD."Title" is distinct from NEW."Title" then
        insert into "TaskHistory" ("TaskId", "UserId", "FieldName", "OldValue", "NewValue", "ChangedAt")
        values (NEW."Id", NEW."OwnerId", 'title', OLD."Title", NEW."Title", NOW());
    end if;
    
    -- log status change
    if OLD."StatusId" is distinct from NEW."StatusId" then
        insert into "TaskHistory" ("TaskId", "UserId", "FieldName", "OldValue", "NewValue", "ChangedAt")
        values (NEW."Id", NEW."OwnerId", 'statusid', OLD."StatusId":: text, NEW."StatusId":: text, NOW());
    end if;
    
    -- log priority change
    if OLD."Priority" is distinct from NEW."Priority" then
        insert into "TaskHistory" ("TaskId", "UserId", "FieldName", "OldValue", "NewValue", "ChangedAt")
        values (NEW."Id", NEW."OwnerId", 'priority', OLD."Priority"::text, NEW."Priority"::text, NOW());
    end if;
    
    -- log description change
    if OLD."Description" is distinct from NEW."Description" then
        insert into "TaskHistory" ("TaskId", "UserId", "FieldName", "OldValue", "NewValue", "ChangedAt")
        values (NEW."Id", NEW."OwnerId", 'description', OLD."Description", NEW."Description", NOW());
    end if;
    
    -- log category change
    if OLD."CategoryId" is distinct from NEW."CategoryId" then
        insert into "TaskHistory" ("TaskId", "UserId", "FieldName", "OldValue", "NewValue", "ChangedAt")
        values (NEW."Id", NEW."OwnerId", 'categoryid', OLD."CategoryId"::text, NEW."CategoryId"::text, NOW());
    end if;
    
    -- log project change
    if OLD."ProjectId" is distinct from NEW."ProjectId" then
        insert into "TaskHistory" ("TaskId", "UserId", "FieldName", "OldValue", "NewValue", "ChangedAt")
        values (NEW."Id", NEW."OwnerId", 'projectid', OLD."ProjectId":: text, NEW."ProjectId":: text, NOW());
    end if;
    
    -- log deadline change
    if OLD."Deadline" is distinct from NEW."Deadline" then
        insert into "TaskHistory" ("TaskId", "UserId", "FieldName", "OldValue", "NewValue", "ChangedAt")
        values (NEW."Id", NEW."OwnerId", 'deadline', OLD."Deadline"::text, NEW."Deadline"::text, NOW());
    end if;
    
    -- log estimated hours change
    if OLD."EstimatedHours" is distinct from NEW."EstimatedHours" then
        insert into "TaskHistory" ("TaskId", "UserId", "FieldName", "OldValue", "NewValue", "ChangedAt")
        values (NEW."Id", NEW."OwnerId", 'estimatedhours', OLD."EstimatedHours"::text, NEW."EstimatedHours"::text, NOW());
    end if;
    
    -- log actual hours change
    if OLD."ActualHours" is distinct from NEW."ActualHours" then
        insert into "TaskHistory" ("TaskId", "UserId", "FieldName", "OldValue", "NewValue", "ChangedAt")
        values (NEW."Id", NEW."OwnerId", 'actualhours', OLD."ActualHours":: text, NEW."ActualHours"::text, NOW());
    end if;
    
    -- log owner change
    if OLD."OwnerId" is distinct from NEW."OwnerId" then
        insert into "TaskHistory" ("TaskId", "UserId", "FieldName", "OldValue", "NewValue", "ChangedAt")
        values (NEW."Id", NEW."OwnerId", 'ownerid', OLD."OwnerId":: text, NEW."OwnerId"::text, NOW());
    end if;
    
    return NEW;
end;
$$ language plpgsql;

create trigger trigger_log_task_changes
    after update on "Tasks"
    for each row
    execute function log_task_changes();



-- cascade delete
drop trigger if exists trigger_delete_task_cascade on "Tasks";
drop function if exists delete_task_related_data();

create or replace function delete_task_related_data()
returns trigger as $$
begin
    -- delete comments
    delete from "Comments" where "TaskId" = OLD."Id";
    
    -- delete history
    delete from "TaskHistory" where "TaskId" = OLD."Id";
    
    -- delete from favorites
    delete from "Favorites" 
    where "EntityTypeId" = (select "Id" from "EntityTypes" where "Name" = 'task')
      and "EntityId" = OLD."Id";
    
    return OLD;
end;
$$ language plpgsql;

create trigger trigger_delete_task_cascade
    before delete on "Tasks"
    for each row
    execute function delete_task_related_data();



-- data validation
drop trigger if exists trigger_validate_task on "Tasks";
drop function if exists validate_task_data();

create or replace function validate_task_data()
returns trigger as $$
begin
    -- check deadline is not earlier than creation date
    if NEW."Deadline" is not null and NEW."CreatedAt" is not null 
       and NEW."Deadline" < NEW."CreatedAt" then
        raise exception 'deadline cannot be earlier than creation date';
    end if;
    
    -- check priority is between 1 and 5
    if NEW."Priority" is not null and (NEW."Priority" < 1 or NEW."Priority" > 5) then
        raise exception 'priority must be between 1 and 5';
    end if;
    
    -- check title is not empty
    if NEW."Title" is null or trim(NEW."Title") = '' then
        raise exception 'title cannot be empty';
    end if;
    
    -- check estimated hours is not negative
    if NEW."EstimatedHours" is not null and NEW."EstimatedHours" < 0 then
        raise exception 'estimated hours cannot be negative';
    end if;
    
    -- check actual hours is not negative
    if NEW."ActualHours" is not null and NEW."ActualHours" < 0 then
        raise exception 'actual hours cannot be negative';
    end if;
    
    return NEW;
end;
$$ language plpgsql;

create trigger trigger_validate_task
    before insert or update on "Tasks"
    for each row
    execute function validate_task_data();



-- update task counters
drop trigger if exists trigger_update_project_task_count on "Tasks";
drop function if exists update_project_task_count();

create or replace function update_project_task_count()
returns trigger as $$
begin
    if TG_OP = 'INSERT' then
        -- increment task count when new task is added
        if NEW."ProjectId" is not null then
            update "Projects"
            set "TaskCount" = coalesce("TaskCount", 0) + 1
            where "Id" = NEW."ProjectId";
        end if;
        
    elsif TG_OP = 'DELETE' then
        -- decrement task count when task is deleted
        if OLD."ProjectId" is not null then
            update "Projects"
            set "TaskCount" = greatest(coalesce("TaskCount", 1) - 1, 0)
            where "Id" = OLD."ProjectId";
        end if;
        
    elsif TG_OP = 'UPDATE' and OLD."ProjectId" is distinct from NEW."ProjectId" then
        -- task moved from one project to another
        if OLD."ProjectId" is not null then
            update "Projects"
            set "TaskCount" = greatest(coalesce("TaskCount", 1) - 1, 0)
            where "Id" = OLD."ProjectId";
        end if;
        
        if NEW."ProjectId" is not null then
            update "Projects"
            set "TaskCount" = coalesce("TaskCount", 0) + 1
            where "Id" = NEW."ProjectId";
        end if;
    end if;
    
    if TG_OP = 'DELETE' then
        return OLD;
    else
        return NEW;
    end if;
end;
$$ language plpgsql;

create trigger trigger_update_project_task_count
    after insert or update or delete on "Tasks"
    for each row
    execute function update_project_task_count();

-- add taskcount column to projects if not exists
alter table "Projects" add column if not exists "TaskCount" int default 0;

-- initialize existing counts
update "Projects" p
set "TaskCount" = (
    select count(*) 
    from "Tasks" t 
    where t."ProjectId" = p."Id"
);