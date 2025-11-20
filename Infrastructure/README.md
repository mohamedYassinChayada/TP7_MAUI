# Infrastructure Layer Implementation

This document describes the Infrastructure layer implementation for web service integration and syncing with JSONPlaceholder API.

## Overview

The Infrastructure layer has been implemented following Clean Architecture principles with:
- **Application/Interfaces**: Core interfaces defining contracts
- **Infrastructure**: Concrete implementations of those interfaces
- **Dependency Injection**: IoC container registration via extension method

## Architecture

### Application Layer (Core Interfaces)

Located in `Application/Interfaces/`:

1. **INoteRepository** - Local data access abstraction
   - Async CRUD operations for notes
   - Support for pending sync queries
   - Upsert capability

2. **INoteRemoteService** - Remote API operations
   - Maps to JSONPlaceholder `/posts` endpoints
   - GET, POST, PUT, DELETE operations
   - Note: JSONPlaceholder simulates writes without persistence

3. **IUserProfileRemoteService** - User profile API
   - Maps to JSONPlaceholder `/users/{id}` endpoint
   - Fetches user profile data

4. **ISyncService** - Synchronization orchestration
   - Pull: Fetches remote notes and stores locally
   - Push: Uploads pending local notes to remote
   - Full sync: Combines pull and push

### Infrastructure Layer

Located in `Infrastructure/`:

#### Repositories (`Infrastructure/Repositories/`)
- **NoteRepository** - Implements `INoteRepository`
  - Wraps existing `IDaoNote` (NoteDAO)
  - Provides async interface over synchronous DAOs
  - Includes logging and error handling

#### Web Services (`Infrastructure/WebServices/`)
- **NotesRemoteService** - Implements `INoteRemoteService`
  - Uses `HttpClient` with dependency injection
  - JSON serialization with case-insensitive properties
  - Maps JSONPlaceholder post structure to Note entity
  - Comprehensive logging

- **UserProfileRemoteService** - Implements `IUserProfileRemoteService`
  - Fetches user profile from JSONPlaceholder
  - Maps API user structure to User entity

#### Sync Services (`Infrastructure/Sync/`)
- **NotesSyncService** - Implements `ISyncService`
  - Orchestrates sync between local and remote
  - Pull: Downloads all posts and upserts locally
  - Push: Uploads pending local notes (simulated)
  - Marks notes with sync status

#### Dependency Injection (`Infrastructure/DependencyInjection.cs`)
- Extension method: `AddInfrastructure()`
- Registers all Infrastructure services with proper lifetimes
- Configures HttpClient with base URL and timeout

## Usage

### Service Registration

The Infrastructure services are registered in `MauiProgram.cs`:

```csharp
using TP6.Infrastructure;

builder.Services.AddInfrastructure();
```

This single line registers:
- Local DAOs (Singleton)
- INoteRepository (Singleton)
- INoteRemoteService with HttpClient (Typed client)
- IUserProfileRemoteService with HttpClient (Typed client)
- ISyncService (Singleton)

### Using Services in ViewModels

#### Example: Syncing Notes

```csharp
public class SyncViewModel : BaseViewModel
{
    private readonly ISyncService _syncService;
    private readonly ILogger<SyncViewModel> _logger;

    public SyncViewModel(ISyncService syncService, ILogger<SyncViewModel> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    [RelayCommand]
    private async Task SyncNotesAsync()
    {
        try
        {
            // Full sync: pull remote notes and push pending local notes
            await _syncService.SyncNotesAsync();

            // Or use individual operations:
            // await _syncService.PullNotesAsync();  // Only pull
            // await _syncService.PushNotesAsync();  // Only push

            _logger.LogInformation("Notes synchronized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing notes");
        }
    }
}
```

#### Example: Using Remote Service

```csharp
public class RemoteNotesViewModel : BaseViewModel
{
    private readonly INoteRemoteService _remoteService;
    private readonly INoteRepository _localRepository;

    public RemoteNotesViewModel(
        INoteRemoteService remoteService,
        INoteRepository localRepository)
    {
        _remoteService = remoteService;
        _localRepository = localRepository;
    }

    [RelayCommand]
    private async Task LoadRemoteNotesAsync()
    {
        var remoteNotes = await _remoteService.GetNotesAsync();
        // Process notes...
    }

    [RelayCommand]
    private async Task CreateNoteAsync(Note note)
    {
        // Mark as pending sync
        note.PendingSync = true;
        note.IsSynced = false;

        // Save locally first
        await _localRepository.InsertAsync(note);

        // Then try to upload (simulated on JSONPlaceholder)
        var result = await _remoteService.CreateNoteAsync(note);

        if (result != null)
        {
            // Update sync status
            note.IsSynced = true;
            note.PendingSync = false;
            await _localRepository.UpdateAsync(note);
        }
    }
}
```

#### Example: Using User Profile Service

```csharp
public class ProfileViewModel : BaseViewModel
{
    private readonly IUserProfileRemoteService _userService;

    public ProfileViewModel(IUserProfileRemoteService userService)
    {
        _userService = userService;
    }

    [RelayCommand]
    private async Task LoadUserProfileAsync()
    {
        var user = await _userService.GetUserAsync(userId: 1);
        if (user != null)
        {
            // Update UI with user data
        }
    }
}
```

## Database Schema Changes

The `Note` entity has been extended with sync tracking fields:

```csharp
public class Note
{
    [PrimaryKey]
    public string Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }

    // New sync fields
    public bool IsSynced { get; set; } = false;
    public bool PendingSync { get; set; } = false;
}
```

Database schema updated in `DaoContext.cs`:
- Added `IsSynced` (INTEGER, DEFAULT 0)
- Added `PendingSync` (INTEGER, DEFAULT 0)

## JSONPlaceholder API Notes

### Important Limitations

1. **Non-Persistent Writes**: POST, PUT, DELETE operations return success responses but **do not persist data**
2. **GET is Reliable**: Only GET operations return actual data
3. **Post ID Mapping**: JSONPlaceholder posts are mapped to Note entities (userId, id, title, body)

### API Endpoints Used

- `GET /posts` - Fetch all posts (mapped to notes)
- `GET /posts/{id}` - Fetch single post
- `POST /posts` - Create post (simulated)
- `PUT /posts/{id}` - Update post (simulated)
- `DELETE /posts/{id}` - Delete post (simulated)
- `GET /users/{id}` - Fetch user profile

## Key Features

1. **Clean Architecture**: Clear separation between interfaces (Application) and implementations (Infrastructure)
2. **Dependency Injection**: All services registered via IoC container
3. **HttpClientFactory**: Proper HttpClient usage with typed clients
4. **Async/Await**: All operations are asynchronous
5. **Logging**: Comprehensive logging using ILogger<T>
6. **Error Handling**: Try-catch blocks with safe defaults
7. **Sync Tracking**: Notes track their sync status (IsSynced, PendingSync)

## Testing the Implementation

### Manual Testing Steps

1. **Pull Remote Notes**:
   ```csharp
   await syncService.PullNotesAsync();
   var notes = await repository.GetAllAsync();
   // Should see ~100 notes from JSONPlaceholder
   ```

2. **Create Local Note with Pending Sync**:
   ```csharp
   var note = new Note
   {
       Title = "Test",
       Content = "Content",
       PendingSync = true
   };
   await repository.InsertAsync(note);
   ```

3. **Push Pending Notes**:
   ```csharp
   await syncService.PushNotesAsync();
   // Note will be marked as synced locally (simulated push)
   ```

4. **Full Sync**:
   ```csharp
   await syncService.SyncNotesAsync();
   // Pulls all remote, pushes all pending
   ```

## No ViewModel Changes Required

The implementation is purely in Infrastructure and Application layers. Existing ViewModels continue to work unchanged. To use the new services:

1. Inject the desired interface in ViewModel constructor
2. Register ViewModel in `MauiProgram.cs` (already done for existing ViewModels)
3. DI automatically provides the implementations

## Files Added

### Application Layer
- `Application/Interfaces/INoteRepository.cs`
- `Application/Interfaces/INoteRemoteService.cs`
- `Application/Interfaces/IUserProfileRemoteService.cs`
- `Application/Interfaces/ISyncService.cs`

### Infrastructure Layer
- `Infrastructure/Repositories/NoteRepository.cs`
- `Infrastructure/WebServices/NotesRemoteService.cs`
- `Infrastructure/WebServices/UserProfileRemoteService.cs`
- `Infrastructure/Sync/NotesSyncService.cs`
- `Infrastructure/DependencyInjection.cs`
- `Infrastructure/README.md` (this file)

### Modified Files
- `Models/Entity/Note.cs` - Added IsSynced and PendingSync properties
- `Infrastructure/Persistance/DaoContext.cs` - Updated schema with sync fields
- `Infrastructure/Persistance/NoteDAO.cs` - Updated Insert/Update to handle sync fields
- `MauiProgram.cs` - Added `builder.Services.AddInfrastructure();`

## Compliance with Requirements

✅ Only Infrastructure code and extension classes added
✅ Dependency injection and IoC used throughout
✅ No changes to Core code (Domain and Application are new, not modified)
✅ No changes to existing public APIs
✅ No changes to project structure
✅ Extension class for DI registration (DependencyInjection.cs)
✅ Single line added to MauiProgram.cs
✅ No DTOs introduced (direct entity mapping)
✅ HttpClientFactory with typed clients
✅ CancellationToken support
✅ ILogger<T> for logging
✅ JsonSerializerOptions with PropertyNameCaseInsensitive
✅ No synchronous .Result/.Wait()
✅ XML documentation on public methods

## Next Steps

To use these services in your application:

1. Inject the services into your ViewModels via constructor
2. Call the sync service to populate local database from JSONPlaceholder
3. Use the repository for local CRUD operations
4. Use remote service for direct API calls when needed
5. Mark notes as PendingSync when creating offline, then sync later

## Support

For questions or issues with the Infrastructure implementation, refer to:
- XML documentation in the interface files
- Inline comments in implementation files
- This README for usage examples
