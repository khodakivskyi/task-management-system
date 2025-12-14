# Порівняльний звіт: Database-First vs Code-First

## Загальна інформація

**Дата створення:** 2024-12-14  
**Міграція:** `20251214174622_InitialCreate`  
**DbContext:** `TaskManagementCodeFirstDbContext`

## Порівняльна таблиця

| Елемент | Database-First (Lab 1) | Code-First (Lab 3) | Співпадає? | Пояснення |
|---------|------------------------|---------------------|------------|-----------|
| **Таблиці** |
| Таблиця Users | ✓ | ✓ | ✓ | Однакова структура |
| Таблиця Tasks | ✓ | ✓ | ✓ | Однакова структура (без StatusId, CategoryId) |
| Таблиця Projects | ✓ | ✓ | ✓ | Однакова структура |
| **Колонки Users** |
| Users.Id | INTEGER (PK, Identity) | INTEGER (PK, Identity) | ✓ | Правильно налаштовано |
| Users.Name | VARCHAR(255) NOT NULL | VARCHAR(255) NOT NULL | ✓ | Правильно налаштовано |
| Users.Surname | VARCHAR(255) NULL | VARCHAR(255) NULL | ✓ | Правильно налаштовано |
| Users.Login | VARCHAR(255) NOT NULL | VARCHAR(255) NOT NULL, UNIQUE | ≈ | Code-First додав UNIQUE constraint |
| Users.PasswordHash | VARCHAR(255) NOT NULL | VARCHAR(255) NOT NULL | ✓ | Правильно налаштовано |
| Users.Salt | VARCHAR(255) NOT NULL | VARCHAR(255) NOT NULL | ✓ | Правильно налаштовано |
| Users.CreatedAt | TIMESTAMP | TIMESTAMP DEFAULT CURRENT_TIMESTAMP | ≈ | Code-First додав default value |
| **Колонки Tasks** |
| Tasks.Id | INTEGER (PK, Identity) | INTEGER (PK, Identity) | ✓ | Правильно налаштовано |
| Tasks.OwnerId | INTEGER (FK) | INTEGER (FK) | ✓ | Правильно налаштовано |
| Tasks.ProjectId | INTEGER (FK, NULL) | INTEGER (FK, NULL) | ✓ | Правильно налаштовано |
| Tasks.StatusId | INTEGER (FK) | ✗ | ✗ | Видалено згідно з вимогами (тільки User, Task, Project) |
| Tasks.CategoryId | INTEGER (FK, NULL) | ✗ | ✗ | Видалено згідно з вимогами (тільки User, Task, Project) |
| Tasks.Title | VARCHAR(50) | VARCHAR(200) | ≈ | Code-First збільшив max length |
| Tasks.Description | VARCHAR(250) | VARCHAR(2000) | ≈ | Code-First збільшив max length |
| Tasks.Priority | INTEGER NULL | INTEGER NULL | ✓ | Правильно налаштовано |
| Tasks.Deadline | TIMESTAMP NULL | TIMESTAMP NULL | ✓ | Правильно налаштовано |
| Tasks.CreatedAt | TIMESTAMP | TIMESTAMP DEFAULT CURRENT_TIMESTAMP | ≈ | Code-First додав default value |
| Tasks.UpdatedAt | TIMESTAMP | TIMESTAMP DEFAULT CURRENT_TIMESTAMP | ≈ | Code-First додав default value |
| Tasks.EstimatedHours | INTEGER | INTEGER DEFAULT 0 | ≈ | Code-First додав default value |
| Tasks.ActualHours | INTEGER | INTEGER DEFAULT 0 | ≈ | Code-First додав default value |
| Tasks.ProgressPercentage | ✗ | NUMERIC (computed) | ✗ | Code-First додав computed column |
| **Колонки Projects** |
| Projects.Id | INTEGER (PK, Identity) | INTEGER (PK, Identity) | ✓ | Правильно налаштовано |
| Projects.OwnerId | INTEGER (FK) | INTEGER (FK) | ✓ | Правильно налаштовано |
| Projects.Name | VARCHAR(100) | VARCHAR(200) | ≈ | Code-First збільшив max length |
| Projects.Description | VARCHAR(500) | VARCHAR(2000) | ≈ | Code-First збільшив max length |
| Projects.StartDate | TIMESTAMP | TIMESTAMP | ✓ | Правильно налаштовано |
| Projects.EndDate | TIMESTAMP | TIMESTAMP | ✓ | Правильно налаштовано |
| Projects.DurationDays | ✗ | INTEGER (computed) | ✗ | Code-First додав computed column |
| Projects.IsActive | ✗ | BOOLEAN (computed) | ✗ | Code-First додав computed column |
| **Foreign Keys** |
| FK_Projects_Users_OwnerId | RESTRICT | RESTRICT | ✓ | Правильно налаштовано |
| FK_Tasks_Users_OwnerId | RESTRICT | RESTRICT | ✓ | Правильно налаштовано |
| FK_Tasks_Projects_ProjectId | SET NULL | SET NULL | ✓ | Правильно налаштовано |
| **Індекси** |
| IX_Users_Login | Можливо не було | UNIQUE + FILTERED | ✗ | Code-First додав unique index з filter |
| IX_Users_Name | Можливо не було | ✓ | ✗ | Code-First додав index |
| IX_Users_Surname | Можливо не було | FILTERED | ✗ | Code-First додав filtered index |
| IX_Users_Name_Surname | Можливо не було | COMPOSITE + FILTERED | ✗ | Code-First додав composite index |
| IX_Users_CreatedAt | Можливо не було | ✓ | ✗ | Code-First додав index |
| IX_Tasks_OwnerId | Можливо було | ✓ | ≈ | Code-First явно налаштував |
| IX_Tasks_ProjectId | Можливо було | FILTERED | ≈ | Code-First додав filter |
| IX_Tasks_Title | Можливо не було | ✓ | ✗ | Code-First додав index |
| IX_Tasks_Priority | Можливо не було | FILTERED | ✗ | Code-First додав filtered index |
| IX_Tasks_Deadline | Можливо не було | FILTERED | ✗ | Code-First додав filtered index |
| IX_Tasks_CreatedAt | Можливо не було | ✓ | ✗ | Code-First додав index |
| IX_Tasks_UpdatedAt | Можливо не було | ✓ | ✗ | Code-First додав index |
| IX_Tasks_OwnerId_CreatedAt | Можливо не було | COMPOSITE | ✗ | Code-First додав composite index |
| IX_Tasks_OwnerId_Priority | Можливо не було | COMPOSITE + FILTERED | ✗ | Code-First додав composite filtered index |
| IX_Tasks_ProjectId_Deadline | Можливо не було | COMPOSITE + FILTERED | ✗ | Code-First додав composite filtered index |
| IX_Projects_OwnerId | Можливо було | ✓ | ≈ | Code-First явно налаштував |
| IX_Projects_Name | Можливо не було | ✓ | ✗ | Code-First додав index |
| IX_Projects_StartDate | Можливо не було | ✓ | ✗ | Code-First додав index |
| IX_Projects_EndDate | Можливо не було | ✓ | ✗ | Code-First додав index |
| IX_Projects_OwnerId_StartDate | Можливо не було | COMPOSITE | ✗ | Code-First додав composite index |
| IX_Projects_StartDate_EndDate | Можливо не було | COMPOSITE | ✗ | Code-First додав composite index |
| **Computed Columns** |
| Tasks.ProgressPercentage | ✗ | ✓ | ✗ | Code-First додав: `CASE WHEN EstimatedHours > 0 THEN ROUND((ActualHours::numeric / EstimatedHours::numeric * 100.0), 2) ELSE 0.0 END` |
| Projects.DurationDays | ✗ | ✓ | ✗ | Code-First додав: `EXTRACT(DAY FROM (EndDate - StartDate))::integer` |
| Projects.IsActive | ✗ | ✓ | ✗ | Code-First додав: `(CURRENT_DATE >= StartDate::date AND CURRENT_DATE <= EndDate::date)` |
| **Default Values** |
| Users.CreatedAt | Можливо не було | CURRENT_TIMESTAMP | ✗ | Code-First додав default value |
| Tasks.CreatedAt | Можливо не було | CURRENT_TIMESTAMP | ✗ | Code-First додав default value |
| Tasks.UpdatedAt | Можливо не було | CURRENT_TIMESTAMP | ✗ | Code-First додав default value |
| Tasks.EstimatedHours | Можливо не було | 0 | ✗ | Code-First додав default value |
| Tasks.ActualHours | Можливо не було | 0 | ✗ | Code-First додав default value |
| **Відсутні елементи в Code-First** |
| Stored Procedures | ✓ | ✗ | ✗ | Code-First не створює stored procedures |
| Views | ✓ (можливо) | ✗ | ✗ | Code-First не створює database views |
| Functions | ✓ (можливо) | ✗ | ✗ | Code-First не створює database functions |
| Triggers | ✓ (можливо) | ✗ | ✗ | Code-First не створює triggers |
| Інші таблиці (Status, Category, Comment, тощо) | ✓ | ✗ | ✗ | Code-First створює тільки User, Task, Project (згідно з вимогами) |

## Висновки

### Переваги Code-First підходу:
1. **Computed Columns**: Додано 3 computed columns для автоматичного обчислення значень
2. **Filtered Indexes**: Додано filtered indexes для оптимізації запитів з nullable полями
3. **Composite Indexes**: Додано composite indexes для оптимізації складних запитів
4. **Default Values**: Явно налаштовані default values для кращої консистентності даних
5. **Explicit Configuration**: Всі relationships та constraints явно налаштовані через Fluent API

### Обмеження Code-First підходу:
1. **Stored Procedures**: Не створюються автоматично, потрібно додавати вручну
2. **Views**: Не створюються автоматично, потрібно додавати вручну
3. **Functions**: Не створюються автоматично, потрібно додавати вручну
4. **Triggers**: Не створюються автоматично, потрібно додавати вручну

### Рекомендації:
- Для складних бізнес-логік використовувати stored procedures та functions, додаючи їх вручну через SQL скрипти
- Для оптимізації складних запитів використовувати views, додаючи їх вручну через SQL скрипти
- Для автоматизації бізнес-логік використовувати triggers, додаючи їх вручну через SQL скрипти або через міграції

