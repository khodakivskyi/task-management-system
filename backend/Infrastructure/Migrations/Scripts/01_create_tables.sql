create table if not exists "Users"
(
    "Id"            serial primary key,
    "Name"          varchar(255) not null,
    "Surname"       varchar(255),
    "Login"         varchar(255) not null,
    "PasswordHash"  varchar(255) not null,
    "Salt"          varchar(255) not null,
    "CreatedAt"     timestamp not null default current_timestamp
);

create table if not exists "Statuses"
(
    "Id"    serial primary key,
    "Name"  varchar(50) not null,
    "Color" varchar(7)
);

create table if not exists "Categories"
(
    "Id"    serial primary key,
    "Name"  varchar(50) not null,
    "Color" varchar(7) not null
);

create table if not exists "Projects"
(
    "Id"          serial primary key,
    "OwnerId"     integer not null,
    "Name"        varchar(100) not null,
    "Description" varchar(500),
    "StartDate"   timestamp not null,
    "EndDate"     timestamp not null,

    constraint "FK_Projects_Owner" foreign key ("OwnerId") references "Users"("Id")
);

create table if not exists "ProjectRoles"
(
    "Id"              serial primary key,
    "Name"            varchar(50) not null,
    "CanCreateTasks"  boolean not null default false,
    "CanEditTasks"    boolean not null default false,
    "CanDeleteTasks"  boolean not null default false,
    "CanAssignTasks"  boolean not null default false,
    "CanManageMembers" boolean not null default false
);

create table if not exists "ProjectMembers"
(
    "Id"         serial primary key,
    "ProjectId"  integer not null,
    "UserId"    integer not null,
    "RoleId"    integer not null,
    "JoinedAt"  timestamp not null default current_timestamp,

    constraint "FK_ProjectMembers_User" foreign key ("UserId") references "Users"("Id"),
    constraint "FK_ProjectMembers_Project" foreign key ("ProjectId") references "Projects"("Id"),
    constraint "FK_ProjectMembers_Role" foreign key ("RoleId") references "ProjectRoles"("Id")
);

create table if not exists "Tasks"
(
    "Id"             serial primary key,
    "OwnerId"        integer not null,
    "StatusId"       integer not null,
    "CategoryId"     integer,
    "ProjectId"      integer,
    "Title"          varchar(50) not null,
    "Description"    varchar(250),
    "Priority"       integer,
    "Deadline"       timestamp,
    "CreatedAt"      timestamp not null default current_timestamp,
    "UpdatedAt"      timestamp not null default current_timestamp,
    "EstimatedHours" integer not null default 0,
    "ActualHours"    integer not null default 0,

    constraint "FK_Tasks_Owner" foreign key ("OwnerId") references "Users"("Id"),
    constraint "FK_Tasks_Status" foreign key ("StatusId") references "Statuses"("Id"),
    constraint "FK_Tasks_Category" foreign key ("CategoryId") references "Categories"("Id"),
    constraint "FK_Tasks_Project" foreign key ("ProjectId") references "Projects"("Id"),
    constraint "Check_Priority" check ("Priority" between 1 and 5)
);

create table if not exists "TaskAssignees"
(
    "Id"      serial primary key,
    "TaskId"  integer not null,
    "UserId"  integer not null,

    constraint "FK_TaskAssignees_User" foreign key ("UserId") references "Users"("Id"),
    constraint "FK_TaskAssignees_Task" foreign key ("TaskId") references "Tasks"("Id")
);

create table if not exists "Comments"
(
    "Id"         serial primary key,
    "TaskId"     integer not null,
    "UserId"     integer not null,
    "Content"    varchar(1000) not null,
    "CreatedAt"  timestamp not null default current_timestamp,

    constraint "FK_Comments_User" foreign key ("UserId") references "Users"("Id"),
    constraint "FK_Comments_Task" foreign key ("TaskId") references "Tasks"("Id")
);

create table if not exists "TaskHistory"
(
    "Id"         serial primary key,
    "TaskId"     integer not null,
    "UserId"     integer not null,
    "FieldName"  varchar(100) not null,
    "OldValue"   text,
    "NewValue"   text,
    "ChangedAt"  timestamp not null default current_timestamp,

    constraint "FK_TaskHistory_Task" foreign key ("TaskId") references "Tasks"("Id"),
    constraint "FK_TaskHistory_User" foreign key ("UserId") references "Users"("Id")
);

create table if not exists "Favorites"
(
    "Id"         serial primary key,
    "UserId"     integer not null,
    "EntityType" varchar(20) not null,
    "EntityId"   integer not null,
    "CreatedAt"  timestamp not null default current_timestamp,

    constraint "FK_Favorites_User" foreign key ("UserId") references "Users"("Id"),
    constraint "Check_EntityType" check ("EntityType" in ('task', 'project'))
);