-- add email column
do $$
begin
    if not exists (
        select 1
        from information_schema.columns
        where table_name = 'Users'
          and column_name = 'Email'
    ) then
        alter table "Users"
            add column "Email" varchar(255) not null default '';
    end if;
end $$;

-- add LastLoginAt column
do $$
begin
    if not exists (
        select 1
        from information_schema.columns
        where table_name = 'Users'
          and column_name = 'LastLoginAt'
    ) then
        alter table "Users"
            add column "LastLoginAt" timestamp;
    end if;
end $$;

-- add IsActive column
do $$
begin
    if not exists (
        select 1
        from information_schema.columns
        where table_name = 'Users'
          and column_name = 'IsActive'
    ) then
        alter table "Users"
            add column "IsActive" boolean not null default true;
    end if;
end $$;

-- add EmailConfirmed column
do $$
begin
    if not exists (
        select 1
        from information_schema.columns
        where table_name = 'Users'
          and column_name = 'EmailConfirmed'
    ) then
        alter table "Users"
            add column "EmailConfirmed" boolean not null default false;
    end if;
end $$;

-- update existing users with dummy emails BEFORE adding unique constraint
update "Users"
set "Email" = lower("Login") || '@example.com'
where "Email" = '' or "Email" is null;

-- add unique constraint on email
do $$
begin
    if not exists (
        select 1
        from pg_constraint
        where conname = 'UQ_Users_Email'
    ) then
        alter table "Users"
            add constraint "UQ_Users_Email" unique ("Email");
    end if;
end $$;

-- create indexes
create index if not exists "IX_Users_Email"
    on "Users" ("Email")
    where "IsActive" = true;

create index if not exists "IX_Users_Login_IsActive"
    on "Users" ("Login")
    where "IsActive" = true;
