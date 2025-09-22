CREATE TABLE [users]
(
    [id]            int IDENTITY(1,1) NOT NULL ,
    [name]          nvarchar(255) NOT NULL ,
    [surname]       nvarchar(255) NULL ,
    [login]         nvarchar(255) NOT NULL ,
    [password_hash] nvarchar(255) NOT NULL ,
    [salt]          nvarchar(255) NOT NULL ,
    [created_at]    datetime NOT NULL DEFAULT GETDATE(),

    CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED ([id] ASC)
    );
GO

CREATE TABLE [statuses]
(
    [id]    int IDENTITY(1,1) NOT NULL ,
    [name]  nvarchar(50) NOT NULL ,
    [color] nvarchar(7) NULL ,

    CONSTRAINT [PK_statuses] PRIMARY KEY CLUSTERED ([id] ASC)
    );
GO

CREATE TABLE [categories]
(
    [id]    int IDENTITY(1,1) NOT NULL ,
    [name]  nvarchar(50) NOT NULL ,
    [color] nvarchar(7) NOT NULL ,

    CONSTRAINT [PK_categories] PRIMARY KEY CLUSTERED ([id] ASC)
    );
GO

CREATE TABLE [projects]
(
    [id]          int IDENTITY(1,1) NOT NULL ,
    [owner_id]    int NOT NULL ,
    [name]        nvarchar(100) NOT NULL ,
    [description] nvarchar(500) NULL ,
    [start_date]  datetime NOT NULL ,
    [end_date]    datetime NOT NULL ,

    CONSTRAINT [PK_projects] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_projects_owner] FOREIGN KEY ([owner_id]) REFERENCES [users]([id])
    );
GO

CREATE TABLE [project_roles]
(
    [id]   int IDENTITY(1,1) NOT NULL ,
    [name] nvarchar(50) NOT NULL ,

    CONSTRAINT [PK_project_roles] PRIMARY KEY CLUSTERED ([id] ASC)
    );
GO

CREATE TABLE [project_members]
(
    [id]         int IDENTITY(1,1) NOT NULL ,
    [project_id] int NOT NULL ,
    [user_id]    int NOT NULL ,
    [role_id]    int NOT NULL ,
    [joined_at]  datetime NOT NULL DEFAULT GETDATE(),

    CONSTRAINT [PK_project_members] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_project_members_user] FOREIGN KEY ([user_id]) REFERENCES [users]([id]),
    CONSTRAINT [FK_project_members_project] FOREIGN KEY ([project_id]) REFERENCES [projects]([id]),
    CONSTRAINT [FK_project_members_role] FOREIGN KEY ([role_id]) REFERENCES [project_roles]([id])
    );
GO

CREATE TABLE [tasks]
(
    [id]              int IDENTITY(1,1) NOT NULL ,
    [owner_id]        int NOT NULL ,
    [status_id]       int NOT NULL ,
    [category_id]     int NULL ,
    [project_id]      int NULL ,
    [title]           nvarchar(50) NOT NULL ,
    [description]     nvarchar(250) NULL ,
    [priority]        int NULL ,
    [deadline]        datetime NULL ,
    [created_at]      datetime NOT NULL DEFAULT GETDATE() ,
    [updated_at]      datetime NOT NULL DEFAULT GETDATE() ,
    [estimated_hours] int NOT NULL DEFAULT 0 ,
    [actual_hours]    int NOT NULL DEFAULT 0 ,

    CONSTRAINT [PK_tasks] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_tasks_owner] FOREIGN KEY ([owner_id]) REFERENCES [users]([id]),
    CONSTRAINT [FK_tasks_status] FOREIGN KEY ([status_id]) REFERENCES [statuses]([id]),
    CONSTRAINT [FK_tasks_category] FOREIGN KEY ([category_id]) REFERENCES [categories]([id]),
    CONSTRAINT [FK_tasks_project] FOREIGN KEY ([project_id]) REFERENCES [projects]([id]),
    CONSTRAINT [check_priority] CHECK (priority BETWEEN 1 AND 5)
    );
GO

CREATE TABLE [task_assignees]
(
    [id]      int IDENTITY(1,1) NOT NULL ,
    [task_id] int NOT NULL ,
    [user_id] int NOT NULL ,

    CONSTRAINT [PK_taskAssignees] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_taskAssignees_user] FOREIGN KEY ([user_id]) REFERENCES [users]([id]),
    CONSTRAINT [FK_taskAssignees_task] FOREIGN KEY ([task_id]) REFERENCES [tasks]([id])
    );
GO

CREATE TABLE [comments]
(
    [id]         int IDENTITY(1,1) NOT NULL ,
    [task_id]    int NOT NULL ,
    [user_id]    int NOT NULL ,
    [content]    nvarchar(1000) NOT NULL ,
    [created_at] datetime NOT NULL DEFAULT GETDATE(),

    CONSTRAINT [PK_comments] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_comments_user] FOREIGN KEY ([user_id]) REFERENCES [users]([id]),
    CONSTRAINT [FK_comments_task] FOREIGN KEY ([task_id]) REFERENCES [tasks]([id])
    );
GO

CREATE INDEX IX_tasks_project_status ON tasks(project_id, status_id);
CREATE INDEX IX_tasks_owner ON tasks(owner_id);
CREATE INDEX IX_tasks_deadline ON tasks(deadline);
CREATE INDEX IX_comments_task ON comments(task_id);
CREATE INDEX IX_project_members_project ON project_members(project_id);
CREATE INDEX IX_taskAssignees_task ON task_assignees(task_id);
GO