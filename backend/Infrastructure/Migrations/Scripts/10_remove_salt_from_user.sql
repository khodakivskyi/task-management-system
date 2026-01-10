do $$
begin
    if exists (
        select 1
        from information_schema.columns
        where table_schema = 'public'
          and lower(table_name) = 'Users'
          and lower(column_name) = 'Salt'
    ) then
        execute 'alter table public.users alter column salt drop not null';

        execute 'update public.users set salt = '''' where salt is null';

        execute 'alter table public.users drop column salt';

        raise notice 'Dropped users.salt column safely';
    else
        raise notice 'Salt column does not exist';
    end if;
end $$;
