# Schema Comparison: Database-First vs Code-First

## Аналіз згенерованої міграції

### Міграція: `20251214174622_InitialCreate`

#### Метод `Up()` - Створення схеми:

**Таблиці:**
1. **Users** - таблиця користувачів
2. **Projects** - таблиця проєктів
3. **Tasks** - таблиця завдань

**Колонки та типи:**
- `Users`: Id (integer, identity), Name (varchar(255)), Surname (varchar(255)), Login (varchar(255), unique), PasswordHash (varchar(255)), Salt (varchar(255)), CreatedAt (timestamp, default CURRENT_TIMESTAMP)
- `Projects`: Id (integer, identity), OwnerId (integer, FK), Name (varchar(200)), Description (varchar(2000)), StartDate (timestamp), EndDate (timestamp), DurationDays (computed), IsActive (computed)
- `Tasks`: Id (integer, identity), OwnerId (integer, FK), ProjectId (integer, FK, nullable), Title (varchar(200)), Description (varchar(2000)), Priority (integer, nullable), Deadline (timestamp, nullable), CreatedAt (timestamp, default CURRENT_TIMESTAMP), UpdatedAt (timestamp, default CURRENT_TIMESTAMP), EstimatedHours (integer, default 0), ActualHours (integer, default 0), ProgressPercentage (computed)

**Computed Columns:**
- `Projects.DurationDays`: `EXTRACT(DAY FROM ("EndDate" - "StartDate"))::integer`
- `Projects.IsActive`: `(CURRENT_DATE >= "StartDate"::date AND CURRENT_DATE <= "EndDate"::date)`
- `Tasks.ProgressPercentage`: `CASE WHEN "EstimatedHours" > 0 THEN ROUND(("ActualHours"::numeric / "EstimatedHours"::numeric * 100.0), 2) ELSE 0.0 END`

**Foreign Keys:**
- `FK_Projects_Users_OwnerId`: Projects.OwnerId → Users.Id (Restrict)
- `FK_Tasks_Projects_ProjectId`: Tasks.ProjectId → Projects.Id (SetNull)
- `FK_Tasks_Users_OwnerId`: Tasks.OwnerId → Users.Id (Restrict)

**Індекси:**
- **Users**: IX_Users_Login (unique, filtered), IX_Users_Name, IX_Users_Surname (filtered), IX_Users_Name_Surname (composite, filtered), IX_Users_CreatedAt
- **Projects**: IX_Projects_OwnerId, IX_Projects_Name, IX_Projects_StartDate, IX_Projects_EndDate, IX_Projects_OwnerId_StartDate (composite), IX_Projects_StartDate_EndDate (composite)
- **Tasks**: IX_Tasks_OwnerId, IX_Tasks_ProjectId (filtered), IX_Tasks_Title, IX_Tasks_Priority (filtered), IX_Tasks_Deadline (filtered), IX_Tasks_CreatedAt, IX_Tasks_UpdatedAt, IX_Tasks_OwnerId_CreatedAt (composite), IX_Tasks_OwnerId_Priority (composite, filtered), IX_Tasks_ProjectId_Deadline (composite, filtered)

#### Метод `Down()` - Відкат міграції:
Видаляє всі таблиці в порядку: Tasks → Projects → Users

### Model Snapshot
Snapshot містить повну конфігурацію моделі на момент створення міграції, включаючи всі relationships, indexes, та computed columns.

## Порівняння з оригінальною БД (Database-First)

### Відмінності:

1. **Computed Columns**: Code-First додав computed columns (DurationDays, IsActive, ProgressPercentage), яких не було в оригінальній схемі
2. **Filtered Indexes**: Code-First додав filtered indexes для nullable полів (Priority, Deadline, ProjectId, Surname)
3. **Composite Indexes**: Code-First додав більше composite indexes для оптимізації запитів
4. **Default Values**: Code-First явно налаштував default values через Fluent API
5. **Delete Behaviors**: Code-First явно налаштував delete behaviors (Restrict, SetNull)

### Відсутні елементи в Code-First:

1. **Stored Procedures**: Code-First не створює stored procedures
2. **Views**: Code-First не створює database views
3. **Functions**: Code-First не створює database functions
4. **Triggers**: Code-First не створює triggers
5. **Інші таблиці**: Code-First створює тільки User, Task, Project (згідно з вимогами)

