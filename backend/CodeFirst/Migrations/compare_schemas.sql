-- Порівняння схем між Database-First та Code-First підходами
-- Виконати ці запити для обох баз даних та порівняти результати

-- 1. Порівняння таблиць
SELECT 
    TABLE_NAME,
    TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'public' 
    AND TABLE_TYPE = 'BASE TABLE'
    AND TABLE_NAME IN ('Users', 'Tasks', 'Projects')
ORDER BY TABLE_NAME;

-- 2. Порівняння колонок для таблиці Users
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'public'
    AND TABLE_NAME = 'Users'
ORDER BY ORDINAL_POSITION;

-- 3. Порівняння колонок для таблиці Tasks
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'public'
    AND TABLE_NAME = 'Tasks'
ORDER BY ORDINAL_POSITION;

-- 4. Порівняння колонок для таблиці Projects
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'public'
    AND TABLE_NAME = 'Projects'
ORDER BY ORDINAL_POSITION;

-- 5. Порівняння Foreign Keys
SELECT
    tc.TABLE_NAME,
    kcu.COLUMN_NAME,
    ccu.TABLE_NAME AS FOREIGN_TABLE_NAME,
    ccu.COLUMN_NAME AS FOREIGN_COLUMN_NAME,
    rc.UPDATE_RULE,
    rc.DELETE_RULE,
    tc.CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS kcu
    ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
    AND tc.TABLE_SCHEMA = kcu.TABLE_SCHEMA
JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS rc
    ON tc.CONSTRAINT_NAME = rc.CONSTRAINT_NAME
JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE AS ccu
    ON rc.UNIQUE_CONSTRAINT_NAME = ccu.CONSTRAINT_NAME
WHERE tc.CONSTRAINT_TYPE = 'FOREIGN KEY'
    AND tc.TABLE_SCHEMA = 'public'
    AND tc.TABLE_NAME IN ('Users', 'Tasks', 'Projects')
ORDER BY tc.TABLE_NAME, kcu.COLUMN_NAME;

-- 6. Порівняння індексів для таблиці Users
SELECT
    i.relname AS index_name,
    a.attname AS column_name,
    ix.indisunique AS is_unique,
    ix.indisprimary AS is_primary,
    pg_get_expr(ix.indpred, ix.indrelid) AS filter_condition
FROM pg_class t
JOIN pg_index ix ON t.oid = ix.indrelid
JOIN pg_class i ON i.oid = ix.indexrelid
LEFT JOIN pg_attribute a ON a.attrelid = t.oid AND a.attnum = ANY(ix.indkey)
WHERE t.relkind = 'r'
    AND t.relname = 'Users'
    AND t.relnamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'public')
ORDER BY i.relname, a.attnum;

-- 7. Порівняння індексів для таблиці Tasks
SELECT
    i.relname AS index_name,
    a.attname AS column_name,
    ix.indisunique AS is_unique,
    ix.indisprimary AS is_primary,
    pg_get_expr(ix.indpred, ix.indrelid) AS filter_condition
FROM pg_class t
JOIN pg_index ix ON t.oid = ix.indrelid
JOIN pg_class i ON i.oid = ix.indexrelid
LEFT JOIN pg_attribute a ON a.attrelid = t.oid AND a.attnum = ANY(ix.indkey)
WHERE t.relkind = 'r'
    AND t.relname = 'Tasks'
    AND t.relnamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'public')
ORDER BY i.relname, a.attnum;

-- 8. Порівняння індексів для таблиці Projects
SELECT
    i.relname AS index_name,
    a.attname AS column_name,
    ix.indisunique AS is_unique,
    ix.indisprimary AS is_primary,
    pg_get_expr(ix.indpred, ix.indrelid) AS filter_condition
FROM pg_class t
JOIN pg_index ix ON t.oid = ix.indrelid
JOIN pg_class i ON i.oid = ix.indexrelid
LEFT JOIN pg_attribute a ON a.attrelid = t.oid AND a.attnum = ANY(ix.indkey)
WHERE t.relkind = 'r'
    AND t.relname = 'Projects'
    AND t.relnamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'public')
ORDER BY i.relname, a.attnum;

-- 9. Перевірка computed columns (stored generated columns)
SELECT
    table_name,
    column_name,
    data_type,
    is_nullable,
    column_default,
    generation_expression
FROM INFORMATION_SCHEMA.COLUMNS
WHERE table_schema = 'public'
    AND table_name IN ('Users', 'Tasks', 'Projects')
    AND generation_expression IS NOT NULL
ORDER BY table_name, ordinal_position;

