# Task Management System

Full-stack task management application with ASP.NET Core, GraphQL, and PostgreSQL.

## 🚀 Quick Start with Docker

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop) installed and running

### Run the Application

```bash
# Clone the repository
git clone https://github.com/khodakivskyi/task-management-system
cd task-management-system

# Copy environment template
cp .env.example .  env
# Edit . env with your database credentials (optional)

# Start everything
docker-compose up

# Open Swagger UI
http://localhost:5000/swagger
```

**That's it!  ** The application will automatically: 
- ✅ Start PostgreSQL database
- ✅ Run all 7 migrations
- ✅ Seed test data (5 users, projects, tasks, etc.)
- ✅ Start backend API

---

## 🛠️ Tech Stack

### Backend
- **ASP.NET Core 10** - Web API framework
- **GraphQL (GraphQL. NET)** - Query language for API
- **PostgreSQL 17** - Database
- **Dapper + ADO.NET** - Data access
- **Custom Migration System** - Database versioning with checksums

### DevOps
- **Docker & Docker Compose** - Containerization
- **GitHub Actions** - CI/CD pipeline
- **Multi-stage Dockerfile** - Optimized images

---

## 📋 Features

- ✅ Task management with projects
- ✅ User authentication & authorization
- ✅ Role-based access control (RBAC)
- ✅ GraphQL API
- ✅ Automated database migrations
- ✅ Docker containerization
- ✅ Health checks
- ✅ Swagger/OpenAPI documentation

---

## 🗄️ Database

**PostgreSQL 17** with:
- Normalized schema (3NF)
- Indexes for performance
- Check constraints for data integrity
- Cascading deletes
- Stored procedures
- Migration history tracking

### Tables
- Users, Projects, Tasks
- Statuses, Categories, ProjectRoles
- Comments, TaskHistory, Favorites

---

## 🧪 Testing

### Access Database

```bash
docker exec -it task-management-system-db psql -U postgres -d task_management_system
```

### Test Data
- 5 Users (john. doe, jane.smith, etc.)
- 5 Statuses (To Do, In Progress, Review, Done, Blocked)
- 5 Categories (Development, Design, Testing, etc.)
- Sample projects and tasks

---

## 🔧 Development

### Local Development (without Docker)

**Prerequisites:**
- .  NET 10 SDK
- PostgreSQL 17

**Steps:**
```bash
# Update .  env with local PostgreSQL
DB_HOST=localhost
DB_PORT=5432
DB_USER=postgres
DB_PASSWORD=your_password
DB_NAME=task_management_system

# Run
cd backend
dotnet restore
dotnet run

# Open
http://localhost:5000/swagger
```

---

## 📚 API Documentation

- **Swagger UI:** http://localhost:5000/swagger
- **GraphQL Playground:** http://localhost:5000/graphql (if enabled)

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
│   ├── backend.csproj
│   ├── backend.sln
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Controllers/               # API Controllers
│   ├── Migrations/                # SQL migration files
│   │   ├── 01_create_tables.sql
│   │   ├── 02_add_indexes.sql
│   │   ├── 03_add_check_constraints.sql
│   │   ├── 04_create_entity_types_and_update_favorites.sql
│   │   ├── 05_create_stored_procedures.sql
│   │   ├── 06_seed_test_data.sql
│   │   └── 07_add_cascades_to_FKs.sql
│   ├── Infrastructure/
│   │   └── Migrations/
│   │       ├── MigrationRunner.cs
│   │       └── MigrationRecord.cs
│   ├── GraphQL/
│   │   └── CustomErrorInfoProvider.cs
│   ├── Models/                    # Data models
│   │   ├── User.cs
│   │   ├── Project.cs
│   │   ├── TaskModel.cs
│   │   ├── Category.cs
│   │   ├── Status.cs
│   │   ├── Comment.cs
│   │   └── ...
│   ├── Repositories/              # Data access layer
│   │   ├── BaseRepository.cs
│   │   ├── UserRepository.cs
│   │   ├── ProjectRepository.cs
│   │   ├── TaskRepository.cs
│   │   └── ...
│   ├── Interfaces/                # Repository interfaces
│   │   ├── IRepository.cs
│   │   └── ITaskRepository.cs
│   ├── Exceptions/                # Custom exceptions
│   │   ├── NotFoundException.cs
│   │   ├── BadRequestException.cs
│   │   ├── UnauthorizedException.cs
│   │   └── ...
│   └── Properties/
│       └── launchSettings.json
├── docker-compose.yml             # Docker configuration
└── README.md
```

---

## 🔐 Environment Variables

See `.env.example` for all available options.

**Required:**
- `DB_USER` - Database username
- `DB_PASSWORD` - Database password
- `DB_NAME` - Database name

**Optional:**
- `DB_PORT` - Database port (default:   5432)
- `BACKEND_PORT` - Backend HTTP port (default: 5000)

---

## 🚀 CI/CD

GitHub Actions workflow automatically:
- ✅ Runs code formatting checks
- ✅ Builds Docker images
- ✅ Runs tests
- ✅ Deploys to staging/production (if configured)

---

## 📖 Architecture

- **Repository Pattern** for data access
- **SOLID principles**
- **Database-first** approach
- **Versioned migrations** with checksums

---

## 📄 License

This project is licensed under the MIT License.

---