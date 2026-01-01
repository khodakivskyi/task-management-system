-- ============================================
-- CHECK Constraints for data validation
-- ============================================

-- ============================================
-- Statuses table constraints
-- ============================================

-- Validate color format (hex color #RRGGBB)
do $$
begin
    if not exists (
        select 1 from pg_constraint 
        where conname = 'Check_Statuses_Color'
    ) then
        alter table "Statuses"
            add constraint "Check_Statuses_Color" 
            check ("Color" is null or ("Color" ~ '^#[0-9A-Fa-f]{6}$'));
    end if;
end $$;

-- ============================================
-- Categories table constraints
-- ============================================

-- Validate color format (hex color #RRGGBB)
do $$
begin
    if not exists (
        select 1 from pg_constraint 
        where conname = 'Check_Categories_Color'
    ) then
        alter table "Categories"
            add constraint "Check_Categories_Color" 
            check ("Color" ~ '^#[0-9A-Fa-f]{6}$');
    end if;
end $$;

-- ============================================
-- Projects table constraints
-- ============================================

-- Validate that end date is after or equal to start date
do $$
begin
    if not exists (
        select 1 from pg_constraint 
        where conname = 'Check_Projects_EndDate'
    ) then
        alter table "Projects"
            add constraint "Check_Projects_EndDate" 
            check ("EndDate" >= "StartDate");
    end if;
end $$;

-- ============================================
-- Tasks table constraints
-- ============================================

-- Validate that estimated hours is non-negative
do $$
begin
    if not exists (
        select 1 from pg_constraint 
        where conname = 'Check_Tasks_EstimatedHours'
    ) then
        alter table "Tasks"
            add constraint "Check_Tasks_EstimatedHours" 
            check ("EstimatedHours" >= 0);
    end if;
end $$;

-- Validate that actual hours is non-negative
do $$
begin
    if not exists (
        select 1 from pg_constraint 
        where conname = 'Check_Tasks_ActualHours'
    ) then
        alter table "Tasks"
            add constraint "Check_Tasks_ActualHours" 
            check ("ActualHours" >= 0);
    end if;
end $$;

-- Validate that updated date is after or equal to created date
do $$
begin
    if not exists (
        select 1 from pg_constraint 
        where conname = 'Check_Tasks_UpdatedAt'
    ) then
        alter table "Tasks"
            add constraint "Check_Tasks_UpdatedAt" 
            check ("UpdatedAt" >= "CreatedAt");
    end if;
end $$;

-- ============================================
-- Comments table constraints
-- ============================================

-- Validate that content is not empty (after trimming whitespace)
do $$
begin
    if not exists (
        select 1 from pg_constraint 
        where conname = 'Check_Comments_Content'
    ) then
        alter table "Comments"
            add constraint "Check_Comments_Content" 
            check (length(trim("Content")) > 0);
    end if;
end $$;

