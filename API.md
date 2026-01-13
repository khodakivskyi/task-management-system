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
    tasks {
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
    task(id: 1) {
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
    deleteTask(input: {
      id: 1
      ownerId: 1
    })
  }
}
```

**Required Fields:**
- `id` (integer) - Task ID to delete
- `ownerId` (integer) - ID of the user requesting deletion (must be the task owner)

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

Search tasks with flexible filters.

**Query:**
```graphql
query {
  taskSearch {
    searchTasks(filter: {
      searchText: "authentication"
      statusId: 2
      priorityMin: 3
      priorityMax: 5
    }) {
      id
      title
      statusName
      priority
      deadline
    }
  }
}
```

**Available Filters (all optional):**
- `userId` - Filter by task owner
- `projectId` - Filter by project
- `statusId` - Filter by status
- `priorityMin` - Minimum priority (1-5)
- `priorityMax` - Maximum priority (1-5)
- `searchText` - Search in title and description

---

## 📁 Projects

### Get All Projects

**Query:**
```graphql
query {
  projects {
    projects {
      id
      name
      description
      ownerId
      startDate
      endDate
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
    project(id: 1) {
      id
      name
      description
      ownerId
      startDate
      endDate
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
    deleteProject(input: {
      id: 1
      ownerId: 1
    })
  }
}
```

**Required Fields:**
- `id` (integer) - Project ID to delete
- `ownerId` (integer) - ID of the user requesting deletion (must be the project owner)

---

### Get Project Statistics

Get comprehensive project statistics.

**Query:**
```graphql
query {
  projectStatistics {
    projectStatistics(projectId: 1) {
      totalTasks
      completedTasks
      inProgressTasks
      overdueTasks
      totalEstimatedHours
      totalActualHours
      efficiencyPercentage
      remainingHours
    }
  }
}
```

**Response:**
```json
{
  "data": {
    "projectStatistics": {
      "projectStatistics": {
        "totalTasks": 15,
        "completedTasks": 8,
        "inProgressTasks": 5,
        "overdueTasks": 2,
        "totalEstimatedHours": 120,
        "totalActualHours": 95,
        "efficiencyPercentage": 79.17,
        "remainingHours": 25
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
    byProjectId(projectId: 1) {
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
    addProjectMember(input: {
      projectId: 1
      userId: 2
      roleId: 2
      requestingUserId: 1
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

**Required Fields:**
- `projectId` (integer) - Project ID
- `userId` (integer) - User ID to add as member
- `roleId` (integer) - Role ID for the member
- `requestingUserId` (integer) - ID of the user making the request (must be project owner)

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
    removeProjectMember(input: {
      projectId: 1
      userId: 2
      requestingUserId: 1
    })
  }
}
```

**Required Fields:**
- `projectId` (integer) - Project ID
- `userId` (integer) - User ID to remove
- `requestingUserId` (integer) - ID of the user making the request (must be project owner)

---

## 🏷️ Categories

### Get All Categories

**Query:**
```graphql
query {
  categories {
    categories {
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
    category(id: 1) {
      id
      name
      description
      color
    }
  }
}
```

---

**Note:** Categories are read-only. Create, update, and delete operations are not supported for categories.

---

## 📊 Statuses

### Get All Statuses

**Query:**
```graphql
query {
  statuses {
    statuses {
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
    status(id: 1) {
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
    byTaskId(taskId: 1) {
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
    deleteComment(input: {
      id: 1
      userId: 1
    })
  }
}
```

**Required Fields:**
- `id` (integer) - Comment ID to delete
- `userId` (integer) - ID of the user requesting deletion (must be the comment author)

---

## 📜 Task History

### Get Task History

**Query:**
```graphql
query {
  taskHistory {
    byTaskId(taskId: 1) {
      id
      taskId
      userId
      fieldName
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
    byUserId(userId: 1) {
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
    removeFavorite(input: {
      userId: 1
      entityTypeId: 1
      entityId: 5
    })
  }
}
```

**Required Fields:**
- `userId` (integer) - User ID
- `entityTypeId` (integer) - Entity type ID (1=Task, 2=Project)
- `entityId` (integer) - ID of the entity to remove from favorites

---

## 📋 Entity Types

### Get All Entity Types

**Query:**
```graphql
query {
  entityTypes {
    entityTypes {
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