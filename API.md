# 📖 API Documentation

## 🚀 Quick Start

### GraphQL Endpoint

```
http://localhost:5000/graphql
```

### GraphQL Playground

Open your browser and navigate to the endpoint above.  The interactive GraphQL playground will be available for testing queries and mutations.

### Authentication

Most operations require authentication. Include the JWT token in the request header:

```http
Authorization: Bearer <your_jwt_token>
```

---

## 🔐 Authentication

### Register

Create a new user account. 

**Mutation:**
```graphql
mutation {
  auth {
    register(input: {
      name: "John"
      surname: "Doe"
      email: "john@example.com"
      login: "johndoe"
      password: "SecurePass123"
    }) {
      token
      expiresAt
      user {
        id
        name
        email
        login
        createdAt
      }
    }
  }
}
```

**Response:**
```json
{
  "data": {
    "auth": {
      "register":  {
        "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.. .",
        "expiresAt": "2026-01-11T12:00:00Z",
        "user": {
          "id": 1,
          "name": "John",
          "email": "john@example.com",
          "login": "johndoe",
          "createdAt": "2026-01-10T12:00:00Z"
        }
      }
    }
  }
}
```

**Password Requirements:**
- Minimum 8 characters
- At least one uppercase letter
- At least one digit
- Maximum 128 characters

---

### Login

Authenticate existing user.

**Mutation:**
```graphql
mutation {
  auth {
    login(input:  {
      loginOrEmail: "john@example.com"
      password: "SecurePass123"
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
}
```

**Response:**
```json
{
  "data": {
    "auth": {
      "login":  {
        "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        "expiresAt":  "2026-01-11T12:00:00Z",
        "user": {
          "id": 1,
          "name": "John",
          "email": "john@example.com",
          "lastLoginAt":  "2026-01-10T12:00:00Z"
        }
      }
    }
  }
}
```

---

### Get Current User

Retrieve authenticated user information.

**Query:**
```graphql
query {
  auth {
    me {
      id
      name
      surname
      email
      login
      createdAt
      lastLoginAt
      emailConfirmed
    }
  }
}
```

**Headers:**
```http
Authorization: Bearer <your_jwt_token>
```

---

## 📋 Tasks

### Get All Tasks

**Query:**
```graphql
query {
  tasks {
    getAll {
      id
      title
      description
      priority
      statusId
      categoryId
      ownerId
      projectId
      deadline
      createdAt
      updatedAt
      estimatedHours
      actualHours
    }
  }
}
```

---

### Get Task by ID

**Query:**
```graphql
query {
  tasks {
    getById(id:  1) {
      id
      title
      description
      priority
      statusId
      categoryId
      ownerId
      projectId
      deadline
      createdAt
      updatedAt
    }
  }
}
```

---

### Create Task

**Mutation:**
```graphql
mutation {
  tasks {
    createTask(input: {
      title: "Implement authentication"
      description: "Add JWT authentication to the API"
      priority: 4
      statusId: 1
      categoryId: 1
      ownerId: 1
      projectId: 1
      deadline: "2026-01-15T00:00:00Z"
      estimatedHours: 8
    }) {
      id
      title
      createdAt
    }
  }
}
```

**Required Fields:**
- `title` (string, 1-500 characters)
- `priority` (integer, 1-5)
- `statusId` (integer)
- `ownerId` (integer)

**Optional Fields:**
- `description` (string)
- `categoryId` (integer)
- `projectId` (integer)
- `deadline` (DateTime)
- `estimatedHours` (integer, >= 0)
- `actualHours` (integer, >= 0)

---

### Update Task

**Mutation:**
```graphql
mutation {
  tasks {
    updateTask(input: {
      id: 1
      title: "Implement JWT authentication"
      description: "Updated description"
      priority: 5
      statusId: 2
      categoryId: 1
      ownerId: 1
      deadline: "2026-01-20T00:00:00Z"
      actualHours: 5
    }) {
      id
      title
      updatedAt
    }
  }
}
```

---

### Delete Task

**Mutation:**
```graphql
mutation {
  tasks {
    deleteTask(id: 1)
  }
}
```

**Response:**
```json
{
  "data": {
    "tasks": {
      "deleteTask": true
    }
  }
}
```

---

### Search Tasks

Search tasks by title, description, or ID.

**Query:**
```graphql
query {
  taskSearch {
    search(searchTerm: "authentication") {
      id
      title
      description
      priority
      statusId
    }
  }
}
```

---

## 📁 Projects

### Get All Projects

**Query:**
```graphql
query {
  projects {
    getAll {
      id
      name
      description
      ownerId
      startDate
      endDate
      createdAt
    }
  }
}
```

---

### Get Project by ID

**Query:**
```graphql
query {
  projects {
    getById(id: 1) {
      id
      name
      description
      ownerId
      startDate
      endDate
      createdAt
    }
  }
}
```

---

### Create Project

**Mutation:**
```graphql
mutation {
  projects {
    createProject(input: {
      name: "Website Redesign"
      description:  "Redesign company website"
      ownerId:  1
      startDate: "2026-01-01T00:00:00Z"
      endDate: "2026-03-31T00:00:00Z"
    }) {
      id
      name
      createdAt
    }
  }
}
```

**Required Fields:**
- `name` (string, 1-255 characters)
- `ownerId` (integer)
- `startDate` (DateTime)
- `endDate` (DateTime, must be >= startDate)

**Optional Fields:**
- `description` (string)

---

### Update Project

**Mutation:**
```graphql
mutation {
  projects {
    updateProject(input: {
      id: 1
      name: "Website Redesign 2.0"
      description: "Complete website overhaul"
      ownerId: 1
      startDate:  "2026-01-01T00:00:00Z"
      endDate: "2026-04-30T00:00:00Z"
    }) {
      id
      name
      updatedAt
    }
  }
}
```

---

### Delete Project

**Mutation:**
```graphql
mutation {
  projects {
    deleteProject(id:  1)
  }
}
```

---

### Get Project Statistics

Get comprehensive project statistics.

**Query:**
```graphql
query {
  projectStatistics {
    getStatistics(projectId: 1) {
      totalTasks
      completedTasks
      inProgressTasks
      overdueTasks
      totalEstimatedHours
      totalActualHours
      completionPercentage
    }
  }
}
```

**Response:**
```json
{
  "data": {
    "projectStatistics": {
      "getStatistics": {
        "totalTasks": 15,
        "completedTasks": 8,
        "inProgressTasks": 5,
        "overdueTasks": 2,
        "totalEstimatedHours": 120,
        "totalActualHours": 95,
        "completionPercentage": 53.33
      }
    }
  }
}
```

---

## 👥 Project Members

### Get Project Members

**Query:**
```graphql
query {
  projectMembers {
    getByProjectId(projectId: 1) {
      id
      projectId
      userId
      roleId
      joinedAt
    }
  }
}
```

---

### Add Project Member

**Mutation:**
```graphql
mutation {
  projectMembers {
    addMember(input: {
      projectId: 1
      userId: 2
      roleId: 2
    }) {
      id
      projectId
      userId
      roleId
      joinedAt
    }
  }
}
```

**Project Roles:**
- `1` - Owner
- `2` - Admin
- `3` - Member
- `4` - Viewer

---

### Remove Project Member

**Mutation:**
```graphql
mutation {
  projectMembers {
    removeMember(id:  1)
  }
}
```

---

## 🏷️ Categories

### Get All Categories

**Query:**
```graphql
query {
  categories {
    getAll {
      id
      name
      description
      color
    }
  }
}
```

---

### Get Category by ID

**Query:**
```graphql
query {
  categories {
    getById(id: 1) {
      id
      name
      description
      color
    }
  }
}
```

---

### Create Category

**Mutation:**
```graphql
mutation {
  categories {
    createCategory(input: {
      name: "Bug Fix"
      description: "Bug fixes and patches"
      color:  "#FF0000"
    }) {
      id
      name
      color
    }
  }
}
```

**Required Fields:**
- `name` (string, 1-100 characters)

**Optional Fields:**
- `description` (string)
- `color` (string, hex format #RRGGBB)

---

### Update Category

**Mutation:**
```graphql
mutation {
  categories {
    updateCategory(input: {
      id:  1
      name: "Critical Bug Fix"
      description: "High priority bug fixes"
      color: "#CC0000"
    }) {
      id
      name
    }
  }
}
```

---

### Delete Category

**Mutation:**
```graphql
mutation {
  categories {
    deleteCategory(id: 1)
  }
}
```

---

## 📊 Statuses

### Get All Statuses

**Query:**
```graphql
query {
  statuses {
    getAll {
      id
      name
      description
      color
    }
  }
}
```

**Default Statuses:**
- To Do
- In Progress
- Review
- Done
- Blocked

---

### Get Status by ID

**Query:**
```graphql
query {
  statuses {
    getById(id: 1) {
      id
      name
      description
      color
    }
  }
}
```

---

## 💬 Comments

### Get Comments by Task ID

**Query:**
```graphql
query {
  comments {
    getByTaskId(taskId: 1) {
      id
      taskId
      userId
      content
      createdAt
    }
  }
}
```

---

### Create Comment

**Mutation:**
```graphql
mutation {
  comments {
    createComment(input:  {
      taskId: 1
      userId: 1
      content: "Great progress on this task!"
    }) {
      id
      taskId
      content
      createdAt
    }
  }
}
```

**Required Fields:**
- `taskId` (integer)
- `userId` (integer)
- `content` (string, not empty after trim)

---

### Update Comment

**Mutation:**
```graphql
mutation {
  comments {
    updateComment(input: {
      id: 1
      taskId: 1
      userId:  1
      content: "Updated comment text"
    }) {
      id
      content
    }
  }
}
```

---

### Delete Comment

**Mutation:**
```graphql
mutation {
  comments {
    deleteComment(id: 1)
  }
}
```

---

## 📜 Task History

### Get Task History

**Query:**
```graphql
query {
  taskHistory {
    getByTaskId(taskId: 1) {
      id
      taskId
      userId
      fieldChanged
      oldValue
      newValue
      changedAt
    }
  }
}
```

**Tracked Fields:**
- Title changes
- Status changes
- Priority changes
- Assignment changes
- Deadline changes

---

## ⭐ Favorites

### Get User Favorites

**Query:**
```graphql
query {
  favorites {
    getByUserId(userId: 1) {
      id
      userId
      entityId
      entityTypeId
      createdAt
    }
  }
}
```

---

### Add to Favorites

**Mutation:**
```graphql
mutation {
  favorites {
    addFavorite(input: {
      userId: 1
      entityId: 5
      entityTypeId: 1
    }) {
      id
      userId
      entityId
      createdAt
    }
  }
}
```

**Entity Types:**
- `1` - Task
- `2` - Project

---

### Remove from Favorites

**Mutation:**
```graphql
mutation {
  favorites {
    removeFavorite(id:  1)
  }
}
```

---

## 📋 Entity Types

### Get All Entity Types

**Query:**
```graphql
query {
  entityTypes {
    getAll {
      id
      name
      description
    }
  }
}
```

**Entity Types:**
- Task
- Project

---

## ❌ Error Handling

### Error Response Format

```json
{
  "errors":  [
    {
      "message": "Task not found",
      "extensions": {
        "code": "NOT_FOUND"
      }
    }
  ]
}
```

### Error Codes

| Code | Description | HTTP Status |
|------|-------------|-------------|
| `VALIDATION_ERROR` | Invalid input data | 400 |
| `UNAUTHORIZED` | Authentication required | 401 |
| `FORBIDDEN` | Insufficient permissions | 403 |
| `NOT_FOUND` | Resource not found | 404 |
| `CONFLICT` | Resource conflict (e.g., duplicate) | 409 |
| `INTERNAL_SERVER_ERROR` | Unexpected server error | 500 |

### Common Validation Errors

**Task Title:**
```json
{
  "errors": [{
    "message": "Task title must be between 1 and 500 characters",
    "extensions": { "code": "VALIDATION_ERROR" }
  }]
}
```

**Task Priority:**
```json
{
  "errors": [{
    "message": "Task priority must be between 1 and 5",
    "extensions":  { "code": "VALIDATION_ERROR" }
  }]
}
```

**Email Format:**
```json
{
  "errors": [{
    "message": "Invalid email format",
    "extensions": { "code": "VALIDATION_ERROR" }
  }]
}
```

**Duplicate Login:**
```json
{
  "errors": [{
    "message": "Unable to create account with provided data",
    "extensions":  { "code": "CONFLICT" }
  }]
}
```

---

## 🔍 Advanced Queries

### Complex Task Query

```graphql
query {
  tasks {
    getAll {
      id
      title
      priority
      deadline
      # Get related entities
      owner:  ownerId
      status: statusId
      category: categoryId
      project: projectId
      
      # Metadata
      createdAt
      updatedAt
      estimatedHours
      actualHours
    }
  }
}
```

### Nested Project Query

```graphql
query {
  projects {
    getById(id: 1) {
      id
      name
      description
      startDate
      endDate
      
      # Get project statistics
      statistics:  id
    }
  }
  
  projectStatistics {
    getStatistics(projectId: 1) {
      totalTasks
      completedTasks
      completionPercentage
    }
  }
}
```

---

## 🚀 Performance Tips

1. **Request only needed fields** - GraphQL allows you to request specific fields
2. **Use search for filtering** - Don't fetch all data and filter client-side
3. **Batch related queries** - Combine multiple queries in one request
4. **Pagination** - For large datasets (future implementation)

---

## 📖 Additional Resources

- **Database Schema:** See [DATABASE.md](DATABASE.md)
- **GraphQL Playground:** http://localhost:5000/graphql
- **Repository:** https://github.com/khodakivskyi/task-management-system

---

## 📝 Notes

- All DateTime fields use ISO 8601 format with UTC timezone
- IDs are auto-generated integers
- Cascading deletes are implemented (e.g., deleting a project deletes its tasks)
- Task history is automatically tracked for certain operations
- Color fields must be in hex format:  `#RRGGBB`

---

**API Version:** 1.0  
**Last Updated:** January 2026