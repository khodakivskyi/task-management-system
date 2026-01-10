do $$
begin
    if exists (
        select 1
        from information_schema.columns
        where table_schema = 'public'
          and table_name = 'Users'
          and column_name = 'Salt'
    ) then
        alter table "Users" alter column "Salt" drop not null;

        update "Users"
        set "Salt" = ''
        where "Salt" is null;

        alter table "Users" drop column "Salt";

        raise notice 'Dropped Users.Salt column';
    else
        raise notice 'Salt column does not exist';
    end if;
end $$;