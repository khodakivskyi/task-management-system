do $$
begin
    if exists (
        select 1
        from information_schema.columns
        where table_schema = 'public'
          and table_name = 'Users'
          and column_name = 'Salt'
    ) then
        raise notice 'Found Salt column, removing... ';
        
        alter table "Users" alter column "Salt" drop not null;
        raise notice 'Dropped NOT NULL constraint';
        
        update "Users" set "Salt" = '' where "Salt" is null;
        raise notice 'Updated NULL values';
        
        alter table "Users" drop column "Salt";
        raise notice 'Dropped Salt column successfully';
    else
        raise notice 'Salt column does not exist, skipping';
    end if;
end $$;