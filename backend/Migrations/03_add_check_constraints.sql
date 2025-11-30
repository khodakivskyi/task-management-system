-- ============================================
-- CHECK Constraints for data validation
-- ============================================

-- ============================================
-- Statuses table constraints
-- ============================================

-- Validate color format (hex color #RRGGBB)
alter table "Statuses"
    add constraint "Check_Statuses_Color" 
    check ("Color" is null or ("Color" ~ '^#[0-9A-Fa-f]{6}$'));

-- ============================================
-- Categories table constraints
-- ============================================

-- Validate color format (hex color #RRGGBB)
alter table "Categories"
    add constraint "Check_Categories_Color" 
    check ("Color" ~ '^#[0-9A-Fa-f]{6}$');

-- ============================================
-- Projects table constraints
-- ============================================

-- Validate that end date is after or equal to start date
alter table "Projects"
    add constraint "Check_Projects_EndDate" 
    check ("EndDate" >= "StartDate");

-- ============================================
-- Tasks table constraints
-- ============================================

-- Validate that estimated hours is non-negative
alter table "Tasks"
    add constraint "Check_Tasks_EstimatedHours" 
    check ("EstimatedHours" >= 0);

-- Validate that actual hours is non-negative
alter table "Tasks"
    add constraint "Check_Tasks_ActualHours" 
    check ("ActualHours" >= 0);

-- Validate that updated date is after or equal to created date
alter table "Tasks"
    add constraint "Check_Tasks_UpdatedAt" 
    check ("UpdatedAt" >= "CreatedAt");

-- ============================================
-- Comments table constraints
-- ============================================

-- Validate that content is not empty (after trimming whitespace)
alter table "Comments"
    add constraint "Check_Comments_Content" 
    check (length(trim("Content")) > 0);

