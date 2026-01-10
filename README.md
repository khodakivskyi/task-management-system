# Task Management System

Full-stack task management application with ASP.NET Core, GraphQL, and PostgreSQL.

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=khodakivskyi_task-management-system&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=khodakivskyi_task-management-system)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=khodakivskyi_task-management-system&metric=bugs)](https://sonarcloud.io/summary/new_code?id=khodakivskyi_task-management-system)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=khodakivskyi_task-management-system&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=khodakivskyi_task-management-system)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=khodakivskyi_task-management-system&metric=coverage)](https://sonarcloud.io/summary/new_code?id=khodakivskyi_task-management-system)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=khodakivskyi_task-management-system&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=khodakivskyi_task-management-system)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=khodakivskyi_task-management-system&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=khodakivskyi_task-management-system)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=khodakivskyi_task-management-system&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=khodakivskyi_task-management-system)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=khodakivskyi_task-management-system&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=khodakivskyi_task-management-system)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=khodakivskyi_task-management-system&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=khodakivskyi_task-management-system)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=khodakivskyi_task-management-system&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=khodakivskyi_task-management-system)

## 🚀 Quick Start with Docker

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop) installed and running

### Run the Application

```bash
# Clone the repository
git clone https://github.com/khodakivskyi/task-management-system
cd task-management-system

# Copy environment template
cp .env.example .env
# Edit .env with your database credentials (optional)

# Start everything
docker-compose up

# Open GraphQL Playground
http://localhost:5000/graphql
```

**That's it!** The application will automatically:
- ✅ Start PostgreSQL database (with UTC timezone)
- ✅ Run all 11 migrations
- ✅ Seed test data (5 users, projects, tasks, etc.)
- ✅ Start backend GraphQL API with JWT authentication

---

## 🛠️ Tech Stack

### Backend
- **ASP.NET Core 10** - Web API framework
- **GraphQL (HotChocolate 13)** - Query language for API
- **PostgreSQL 17** - Database with timestamptz (UTC)
- **Dapper + ADO.NET** - Data access
- **Npgsql** - PostgreSQL provider for .NET
- **BCrypt.NET** - Password hashing
- **JWT Authentication** - Secure token-based auth
- **Custom Migration System** - Database versioning with checksums

### Testing & Quality
- **xUnit** - Modern testing framework
- **Unit Tests** - Service and helper layer tests
- **Integration Tests** - Repository and database tests
- **Coverlet** - Cross-platform code coverage
- **SonarCloud** - Continuous code quality inspection
  - Static code analysis
  - Security vulnerability detection
  - Code smell detection
  - Technical debt tracking

### DevOps & Infrastructure
- **Docker & Docker Compose** - Containerization and orchestration
- **Multi-stage Dockerfile** - Optimized production images
- **Health checks** - Container health monitoring
- **GitHub Actions** - CI/CD pipeline automation
- **Environment-based Configuration** - .env file support
- **Automated Migrations** - Database versioning on startup

---

## 📋 Features

### Core Features
- ✅ **Task Management** - Create, update, delete tasks with priorities and deadlines
- ✅ **Project Organization** - Organize tasks into projects
- ✅ **User Authentication** - JWT-based registration and login
- ✅ **Role-Based Access Control (RBAC)** - Project roles with granular permissions
- ✅ **Comments** - Add comments to tasks
- ✅ **Task History** - Track all changes to tasks
- ✅ **Favorites** - Mark tasks and projects as favorites
- ✅ **Task Search** - Advanced filtering and search
- ✅ **Project Statistics** - Real-time project progress tracking

### Technical Features
- ✅ **GraphQL API** - Flexible and efficient data querying
- ✅ **Automated Database Migrations** - Version-controlled schema changes
- ✅ **UTC Timezone Handling** - Consistent datetime storage and handling
- ✅ **Docker Containerization** - Easy deployment
- ✅ **Health Checks** - Container monitoring
- ✅ **Custom Error Handling** - Graceful error responses
- ✅ **Unit Tests** - Tested business logic

---

## 🗄️ Database

**PostgreSQL 17** with:
- **Normalized schema (3NF)** - Clean data structure
- **Indexes** - Optimized query performance
- **Check constraints** - Data integrity validation
- **Cascading deletes** - Referential integrity
- **Stored procedures** - Complex business logic
- **Triggers** - Automated task history tracking
- **timestamptz columns** - Proper timezone handling (UTC)
- **Migration history tracking** - Version control with checksums

### Tables
- **Users** - User accounts with authentication
- **Projects** - Project containers for tasks
- **Tasks** - Main task entities with deadlines and priorities
- **ProjectMembers** - Project membership with roles
- **ProjectRoles** - Granular permissions (create, edit, delete, assign, manage)
- **TaskAssignees** - Task assignments to users
- **Statuses** - Task statuses (To Do, In Progress, Review, Done, Blocked)
- **Categories** - Task categorization (Development, Design, Testing, etc.)
- **Comments** - Task comments
- **TaskHistory** - Audit trail of all task changes
- **Favorites** - User favorites (tasks and projects)
- **EntityTypes** - Type definitions for polymorphic relations

### Migrations
1. `01_create_tables.sql` - Initial schema
2. `02_add_indexes.sql` - Performance indexes
3. `03_add_check_constraints.sql` - Data validation
4. `04_create_entity_types_and_update_favorites.sql` - Entity types
5. `05_create_stored_procedures.sql` - Task creation procedure
6. `06_add_cascades_to_FKs.sql` - Cascade delete rules
7. `07_add_triggers.sql` - Task history triggers
8. `08_stored_procedures.sql` - Advanced stored procedures
9. `09_add_auth_fields.sql` - Authentication fields
10. `10_remove_salt_from_user.sql` - Password hash refactoring
11. `11_convert_to_timestamptz.sql` - UTC timezone support
12. `99_seed_test_data.sql` - Test data

---

## 🧪 Testing

### Access Database

```bash
# Connect to PostgreSQL
docker exec -it task-management-system-db psql -U postgres -d task_management_system

# Example queries
SELECT * FROM "Users";
SELECT * FROM "Tasks" ORDER BY "CreatedAt" DESC LIMIT 10;
SELECT * FROM "__MigrationHistory" ORDER BY "AppliedAt" DESC;
```

### Test Data
The system automatically seeds test data on first run:
- **5 Users** - john.doe, jane.smith, bob.johnson, alice.williams, charlie.brown
- **5 Statuses** - To Do, In Progress, Review, Done, Blocked
- **5 Categories** - Development, Design, Testing, Documentation, Bug Fix
- **3 Project Roles** - Admin, Member, Viewer (with different permissions)
- **Sample projects and tasks** - Realistic data for testing

### Unit Tests

```bash
# Run all tests
cd backend
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

Test coverage includes:
- AuthHelper tests (token generation, validation)
- ValidationHelper tests (input validation)
- AuthService tests (registration, login)

---

## 🔧 Development

### Local Development (without Docker)

**Prerequisites:**
- .NET 10 SDK
- PostgreSQL 17

**Steps:**
```bash
# Update .env with local PostgreSQL
DB_HOST=localhost
DB_PORT=5432
DB_USER=postgres
DB_PASSWORD=your_password
DB_NAME=task_management_system
JWT_SECRET=your-super-secret-jwt-key-min-32-chars
JWT_ISSUER=TaskManagementSystem
JWT_AUDIENCE=TaskManagementSystem
JWT_EXPIRATION_HOURS=24

# Restore and run
cd backend
dotnet restore
dotnet run

# Open GraphQL playground
http://localhost:5000/graphql
```

### Development with Hot Reload

```bash
cd backend
dotnet watch run
```

Changes to `.cs` files will automatically reload the application.

---

## 📚 API Documentation

### GraphQL Playground
- **URL:** http://localhost:5000/graphql
- **Interactive IDE** for exploring the API
- **Auto-completion** and documentation

### Example Queries

**Register a new user:**
```graphql
mutation {
  register(input: {
    name: "Test"
    surname: "User"
    email: "test@example.com"
    login: "testuser"
    password: "SecurePassword123!"
  }) {
    token
    expiresAt
    user {
      id
      name
      email
    }
  }
}
```

**Login:**
```graphql
mutation {
  login(input: {
    loginOrEmail: "testuser"
    password: "SecurePassword123!"
  }) {
    token
    expiresAt
    user {
      id
      name
      email
      lastLoginAt
    }
  }
}
```

**Get tasks (requires authentication):**
```graphql
query {
  tasks {
    id
    title
    description
    priority
    deadline
    createdAt
    updatedAt
  }
}
```

**Create a task:**
```graphql
mutation {
  createTask(input: {
    ownerId: 1
    statusId: 1
    title: "New Task"
    description: "Task description"
    priority: 3
    deadline: "2026-12-31T23:59:59Z"
    estimatedHours: 8
    actualHours: 0
  }) {
    id
    title
    createdAt
  }
}
```

### Authentication
Add JWT token to HTTP headers:
```json
{
  "Authorization": "Bearer YOUR_JWT_TOKEN_HERE"
}
```

---

## 🐳 Docker Commands

```bash
# Start in background
docker-compose up -d

# View logs
docker-compose logs -f

# Stop everything
docker-compose down

# Stop + delete database (⚠️ deletes all data!)
docker-compose down -v

# Rebuild after code changes
docker-compose up --build

# Restart only backend
docker-compose restart backend
```

---

## 🏗️ Project Structure

```
task-management-system/
├── backend/
│   ├── Dockerfile
│   ├── docker-entrypoint.sh
│   ├── backend.csproj
│   ├── backend.sln
│   ├── Program.cs                 # Application entry point
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   │
│   ├── Configuration/             # App configuration
│   │   ├── AppOptions.cs
│   │   ├── DatabaseOptions.cs     # DB connection with UTC timezone
│   │   ├── JwtOptions.cs
│   │   └── ConfigurationLoader.cs
│   │
│   ├── Infrastructure/
│   │   ├── Migrations/
│   │   │   ├── Scripts/           # SQL migration files
│   │   │   │   ├── 01_create_tables.sql
│   │   │   │   ├── 02_add_indexes.sql
│   │   │   │   ├── 03_add_check_constraints.sql
│   │   │   │   ├── 04_create_entity_types_and_update_favorites.sql
│   │   │   │   ├── 05_create_stored_procedures.sql
│   │   │   │   ├── 06_add_cascades_to_FKs.sql
│   │   │   │   ├── 07_add_triggers.sql
│   │   │   │   ├── 08_stored_procedures.sql
│   │   │   │   ├── 09_add_auth_fields.sql
│   │   │   │   ├── 10_remove_salt_from_user.sql
│   │   │   │   ├── 11_convert_to_timestamptz.sql
│   │   │   │   └── 99_seed_test_data.sql
│   │   │   ├── MigrationRunner.cs
│   │   │   ├── MigrationRecord.cs
│   │   │   ├── MigrationExecutor.cs
│   │   │   └── MigrationStartup.cs
│   │   │
│   │   └── Repositories/          # Data access layer
│   │       ├── BaseRepository.cs
│   │       ├── UserRepository.cs
│   │       ├── TaskRepository.cs
│   │       ├── ProjectRepository.cs
│   │       ├── CommentRepository.cs
│   │       ├── FavoriteRepository.cs
│   │       ├── TaskHistoryRepository.cs
│   │       └── Interfaces/
│   │           ├── IRepository.cs
│   │           ├── ITaskRepository.cs
│   │           └── ...
│   │
│   ├── GraphQL/                   # GraphQL API
│   │   ├── RootQuery.cs
│   │   ├── RootMutation.cs
│   │   ├── Auth/
│   │   │   ├── AuthQuery.cs
│   │   │   └── AuthMutation.cs
│   │   ├── Tasks/
│   │   │   ├── TasksQuery.cs
│   │   │   ├── TasksMutation.cs
│   │   │   └── Inputs/
│   │   ├── Projects/
│   │   ├── Comments/
│   │   ├── Favorites/
│   │   ├── ProjectMembers/
│   │   ├── TaskHistory/
│   │   ├── TaskSearch/
│   │   └── Extensions/
│   │       └── GraphQLErrorFilter.cs
│   │
│   ├── Services/                  # Business logic layer
│   │   ├── TaskService.cs
│   │   ├── ProjectService.cs
│   │   ├── AuthService.cs
│   │   ├── CommentService.cs
│   │   ├── FavoriteService.cs
│   │   └── Interfaces/
│   │       ├── ITaskService.cs
│   │       └── ...
│   │
│   ├── Helpers/                   # Helper utilities
│   │   ├── AuthHelper.cs          # JWT & password helpers
│   │   ├── ValidationHelper.cs    # Input validation
│   │   ├── TaskHelper.cs
│   │   └── ...
│   │
│   ├── Models/                    # Data models & DTOs
│   │   ├── User.cs
│   │   ├── Project.cs
│   │   ├── TaskModel.cs
│   │   ├── Comment.cs
│   │   ├── DTOs/
│   │   │   ├── RegisterRequest.cs
│   │   │   ├── LoginRequest.cs
│   │   │   └── AuthResponse.cs
│   │   └── ...
│   │
│   ├── Exceptions/                # Custom exceptions
│   │   ├── NotFoundException.cs
│   │   ├── BadRequestException.cs
│   │   ├── UnauthorizedException.cs
│   │   ├── ForbiddenException.cs
│   │   ├── ConflictException.cs
│   │   └── ValidationException.cs
│   │
│   ├── backend.Tests/             # Unit tests
│   │   ├── Services/
│   │   │   └── AuthServiceTests.cs
│   │   └── Helpers/
│   │       ├── AuthHelperTests.cs
│   │       └── ValidationHelperTests.cs
│   │
│   └── Properties/
│       └── launchSettings.json
│
├── .gitattributes                 # Git line endings config
├── docker-compose.yml             # Docker orchestration
├── .env.example                   # Environment template
└── README.md
```

---

## 🔐 Environment Variables

See `.env.example` for all available options.

**Required:**
- `DB_USER` - Database username
- `DB_PASSWORD` - Database password
- `DB_NAME` - Database name
- `JWT_SECRET` - JWT signing key (min 32 characters)
- `JWT_ISSUER` - JWT issuer (e.g., "TaskManagementSystem")
- `JWT_AUDIENCE` - JWT audience (e.g., "TaskManagementSystem")

**Optional:**
- `DB_HOST` - Database host (default: localhost)
- `DB_PORT` - Database port (default: 5432)
- `BACKEND_PORT` - Backend HTTP port (default: 5000)
- `JWT_EXPIRATION_HOURS` - Token expiration time (default: 24)

---

## 📖 Architecture

### Design Patterns
- **Repository Pattern** - Clean separation of data access
- **Service Layer Pattern** - Business logic isolation
- **Dependency Injection** - Loose coupling
- **SOLID Principles** - Maintainable and extensible code

### Key Architectural Decisions
1. **GraphQL over REST** - Flexible, efficient data fetching
2. **Database-first approach** - SQL migrations for full control
3. **UTC timezone everywhere** - Consistent datetime handling
4. **JWT authentication** - Stateless, scalable auth
5. **Custom migration system** - Checksums prevent drift
6. **Npgsql best practices** - Proper PostgreSQL integration

### Layers
```
┌─────────────────────────────────────┐
│         GraphQL API Layer           │  ← Client requests
├─────────────────────────────────────┤
│       Service Layer (Logic)         │  ← Business rules
├─────────────────────────────────────┤
│    Repository Layer (Data Access)   │  ← SQL queries
├─────────────────────────────────────┤
│    PostgreSQL Database (UTC)        │  ← Data storage
└─────────────────────────────────────┘
```

### Timezone Handling
- **Database**: All `timestamptz` columns store UTC
- **Backend**: All `DateTime` values use `DateTime.UtcNow`
- **API**: ISO 8601 format with UTC timezone (`2026-01-10T16:30:00Z`)
- **Frontend**: Convert to user's local timezone

This ensures consistent datetime handling across the entire system.

---

## 🚀 CI/CD Pipeline

### GitHub Actions Workflow

The project uses **GitHub Actions** for continuous integration on every pull request to master.

**Pipeline Steps:**

1. **Code Formatting Check**
   ```bash
   dotnet format --verify-no-changes
   ```
   - Ensures consistent code style (EditorConfig rules)
   - Validates indentation, spacing, naming conventions
   - Fails fast if formatting issues detected

2. **Build & Restore**
   - Restores NuGet packages with caching
   - Compiles solution in Release configuration
   - Validates project structure and dependencies
   - Multi-target framework support

3. **Unit Tests with Coverage**
   ```bash
   dotnet test --collect:"XPlat Code Coverage"
   ```
   - Runs all xUnit tests
   - Generates code coverage reports (OpenCover format)
   - Uses coverlet for cross-platform coverage
   - Excludes test files and migrations from coverage

4. **SonarCloud Analysis**
   - **Static Code Analysis** - Detects bugs and code smells
   - **Security Scanning** - Identifies vulnerabilities
   - **Code Coverage Tracking** - Monitors test coverage
   - **Technical Debt Assessment** - Measures maintainability
   - **Quality Gate Enforcement** - Blocks poor quality code

**Configuration:** `.github/workflows/ci.yml`

### 📊 SonarCloud Dashboard

View live metrics: [SonarCloud Dashboard](https://sonarcloud.io/summary/new_code?id=khodakivskyi_task-management-system)

**Key Metrics:**
- **Quality Gate** - Overall pass/fail status
- **Coverage** - Percentage of code tested
- **Bugs** - Reliability issues
- **Vulnerabilities** - Security issues
- **Code Smells** - Maintainability issues
- **Technical Debt** - Time to fix issues
- **Duplications** - Code duplication percentage
- **Maintainability Rating** - A to E scale
- **Security Rating** - A to E scale
- **Reliability Rating** - A to E scale

### ✅ Quality Gates

All PRs must pass:
- ✅ **Code formatting** - `dotnet format` standards
- ✅ **Successful build** - Compiles without errors
- ✅ **All tests passing** - 100% test success rate
- ✅ **SonarCloud quality gate** - Meets quality standards
  - 0 bugs introduced
  - 0 vulnerabilities introduced
  - 0 security hotspots
  - Code coverage ≥ target threshold
  - Technical debt ratio ≤ 5%
  - Duplication ≤ 3%

### 🔄 Workflow Triggers

```yaml
on:
  pull_request:
    branches: [ master ]
```

CI runs automatically on:
- Pull requests to master branch
- Updates to existing pull requests
- Manual workflow dispatch (if enabled)

---

## 📄 License

This project is licensed under the MIT License.

---

## 👨‍💻 Author

Developed as a database course project demonstrating:
- PostgreSQL advanced features (stored procedures, triggers, indexes)
- Modern .NET development practices
- GraphQL API design
- Docker containerization
- UTC timezone handling best practices