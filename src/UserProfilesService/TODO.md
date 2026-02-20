# UserProfilesService — Development TODO

## Priority 1: Testing Infrastructure ✅ COMPLETED

### 1.1. Test Project Setup
- [x] Create `UserProfilesService.Testing` project (xUnit)
- [x] Add packages:
  - `Microsoft.NET.Test.Sdk`
  - `xunit` + `xunit.runner.visualstudio`
  - `Shouldly` (readable assertions)
  - `Testcontainers.MongoDb` (MongoDB in Docker for tests)
  - `MongoDB.Driver` (for connection string manipulation)
- [x] Add project reference to `UserProfilesService.Persistence`

### 1.2. Basic Test Infrastructure
- [x] Create `MongoDbFixture` for MongoDB container lifecycle
- [x] Create `MongoDbCollection` for xUnit collection fixture
---

## Priority 2: Data Model & Repository (TDD) ✅ IN PROGRESS

### 2.1. Entity Model — ProfileDocument
- [x] `ProfileDocument` exists in `UserProfilesService.Persistence/Documents/`
  - `Id` (string)
  - `Username` (string)
  - `Email` (string)
  - `CreatedAt` (DateTime)
  - `Characters` (CharacterDocument[]?)
- [ ] Add `UserId` field for Keycloak user reference
- [ ] Add `UpdatedAt` field
- [ ] Add `IsDeleted` flag for soft delete
- [ ] Add validation tests for required fields

### 2.2. EF Core DbContext
- [x] `MongoDbContext` exists
- [x] Configured with `MongoDB.EntityFrameworkCore`
- [x] Integration tests with MongoDB container working

### 2.3. Repository Pattern ✅ COMPLETED
- [x] Create `IRepository<T>` interface:
  - `Task<T> AddAsync(T entity, CancellationToken)`
  - `Task<T?> GetByIdAsync(string id, CancellationToken)`
  - `Task<T> UpdateAsync(T entity, CancellationToken)`
  - `Task DeleteAsync(string id, CancellationToken)`
  - `IAsyncEnumerable<T> GetAllAsync(CancellationToken)`
- [x] Implement `ProfileRepository : IRepository<ProfileDocument>`
- [x] Integration tests for all repository methods:
  - `add_call_should_create_new_record_if_id_unique`
  - `get_by_id_should_return_null_if_not_found`
  - `update_call_should_modify_existing_record`
  - `delete_call_should_remove_record`
  - `delete_call_should_not_fail_for_non_existent_id`
  - `get_all_should_return_all_documents`

---

## Priority 3: Keycloak Event Handlers

### 3.1. AccountCreated Event Handling
- [ ] Update `AccountCreated` event with `UserId`, `Username`, `Email`
- [ ] Create `OnAccountCreatedHandler`:
  - Auto-create `UserProfile` on event
  - Error logging
- [ ] Unit test with mocked repository
- [ ] Integration test with RabbitMQ + MongoDB

### 3.2. AccountDeleted Event Handling
- [ ] Update `AccountDeleted` event with `UserId`
- [ ] Create `OnAccountDeletedHandler`:
  - Soft delete profile
  - Logging
- [ ] Unit test with mocked repository
- [ ] Integration test

---

## Priority 4: API Endpoints for Profile Management

### 4.1. GET /user/profiles/{userId} — Get Profile
- [ ] Write integration test for endpoint
- [ ] Create DTO `UserProfileResponse`
- [ ] Implement endpoint in `UserProfileController`
- [ ] Add JWT authorization
- [ ] Test: successful retrieval
- [ ] Test: not found (404)
- [ ] Test: unauthorized (401)

### 4.2. PUT /user/profiles/{userId} — Update Profile
- [ ] Write integration test
- [ ] Create DTO `UpdateUserProfileRequest`:
  - `Username` (optional)
  - `Email` (optional)
  - `Bio` (optional)
  - `AvatarUrl` (optional)
- [ ] Implement endpoint
- [ ] Input validation
- [ ] Test: successful update
- [ ] Test: partial update (PATCH logic)
- [ ] Test: validation errors (400)

### 4.3. GET /user/profiles/me — Current User Profile
- [ ] Write test
- [ ] Implement endpoint with JWT `UserId` extraction
- [ ] Test: successful retrieval

---

## Priority 5: Additional Features

### 5.1. Caching
- [ ] Add `IDistributedCache` (Redis via Aspire)
- [ ] Cache frequently read profiles
- [ ] Cache invalidation on update

### 5.2. Pagination & Search
- [ ] Write tests first
- [ ] Endpoint `GET /user/profiles/search?query=&page=&pageSize=`
- [ ] Implement search by username/email

### 5.3. Audit Log
- [ ] Profile change logging
- [ ] Entity `UserProfileAudit`
- [ ] Admin endpoint for change history

---

## Priority 6: CI/CD & Documentation

### 6.1. CI Pipeline ✅ PARTIALLY COMPLETED
- [x] NUKE build with `CoreTests` target
- [x] GitHub Actions attribute configured
- [ ] Ensure test containers run in CI (Docker/Podman)
- [ ] Code coverage report

### 6.2. API Documentation
- [ ] Update Scalar/OpenAPI specification
- [ ] Add request/response examples
- [ ] Document error codes

### 6.3. README
- [ ] Describe how to run tests
- [ ] Document dependent services
- [ ] Configuration examples

---

## Notes

### Test Containers
Using **Testcontainers** for Docker/Podman container management in tests.

**Podman Support:** Testcontainers supports Podman via Docker socket compatibility.

On Windows with Podman:
```powershell
$env:DOCKER_HOST="npipe:////./pipe/podman"
dotnet test
```

### Fixture Example
```csharp
public sealed class MongoDbFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _container;

    public MongoDbFixture()
    {
        _container = new MongoDbBuilder("mongo:8").Build();
    }

    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
```

### TDD Cycle
1. **Red**: Write a failing test
2. **Green**: Write minimal code to pass the test
3. **Refactor**: Improve code while keeping tests green

### Keycloak Events
Keycloak sends events to RabbitMQ via plugin. Ensure:
- Event format is compatible with our models
- Exchange/queue properly configured in AppHost
- Events contain `UserId` for profile linking

### Current Test Structure
```
UserProfilesService.Testing/
├── Fixtures/
│   └── MongoDbFixture.cs
├── RepositoryTests/
│   ├── MongoDbCollection.cs
│   └── repository_should_handle_data_correctly.cs
```

### Running Tests
```bash
# Via NUKE
./build.cmd Test

# Direct
dotnet test src/UserProfilesService.Testing/UserProfilesService.Testing.csproj
```
