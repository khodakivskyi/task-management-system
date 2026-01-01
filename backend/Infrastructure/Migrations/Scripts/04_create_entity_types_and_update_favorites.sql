-- ============================================
-- Create EntityTypes table
-- ============================================

create table if not exists "EntityTypes"
(
    "Id"    serial primary key,
    "Name"  varchar(20) not null unique
);

insert into "EntityTypes" ("Name") values ('task')
on conflict do nothing;

insert into "EntityTypes" ("Name") values ('project')
on conflict do nothing;

-- ============================================
-- Update Favorites table to use EntityTypes
-- ============================================

do $$
begin
    if exists (
        select 1 from pg_constraint 
        where conname = 'Check_EntityType'
    ) then
        alter table "Favorites" drop constraint "Check_EntityType";
    end if;
end $$;

alter table "Favorites" add column if not exists "EntityTypeId" integer;

alter table "Favorites" alter column "EntityTypeId" set not null;

do $$
begin
    if not exists (
        select 1 from pg_constraint 
        where conname = 'FK_Favorites_EntityType'
    ) then
        alter table "Favorites"
            add constraint "FK_Favorites_EntityType" 
            foreign key ("EntityTypeId") references "EntityTypes"("Id");
    end if;
end $$;

alter table "Favorites" drop column if exists "EntityType";

-- ============================================
-- Add index on EntityTypeId
-- ============================================

create index if not exists "IX_Favorites_EntityTypeId" on "Favorites"("EntityTypeId");

