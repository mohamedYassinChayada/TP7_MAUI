# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET MAUI cross-platform note-taking application (TP6) that targets Android, iOS, macOS Catalyst, and Windows. The app allows users to create and view notes with a local SQLite database and includes API integration with JSONPlaceholder for demonstration purposes.

## Build and Run Commands

### Build the project
```bash
dotnet build
```

### Build for specific platform
```bash
dotnet build -f net8.0-android
dotnet build -f net8.0-ios
dotnet build -f net8.0-maccatalyst
dotnet build -f net8.0-windows10.0.19041.0
```

### Run the app (Windows)
```bash
dotnet build -t:Run -f net8.0-windows10.0.19041.0
```

### Clean build artifacts
```bash
dotnet clean
```

### Restore NuGet packages
```bash
dotnet restore
```

## Architecture

### Clean Architecture with Layered Pattern

The codebase follows Clean Architecture principles with clear separation of concerns:

**Application Layer** (`Application/Interfaces/`)
- Core interface definitions following Clean Architecture
- `INoteRepository`: Local data access abstraction with async operations
- `INoteRemoteService`: Remote API operations contract (JSONPlaceholder integration)
- `IUserProfileRemoteService`: User profile API contract
- `ISyncService`: Sync orchestration between local and remote
- All interfaces support CancellationToken for async operations

**Domain/Models Layer** (`Models/`)
- `Entity/`: Domain entities (Note, User) with SQLite attributes
  - Note entity includes sync tracking: `IsSynced`, `PendingSync`
- `Business/`: Business logic services (legacy services, prefer using Application interfaces)
  - `NoteService`: Singleton service managing notes with local database (includes default notes initialization)
  - `NotesApiService`: HTTP service for API operations (JSONPlaceholder integration)
  - `UserService`: Business logic for user management

**Infrastructure Layer** (`Infrastructure/`)
- `Persistance/`: Data access implementations
  - `DaoContext`: Singleton database context managing SQLite connection and schema initialization
  - `NoteDAO`, `UserDAO`: Data access objects implementing DAO pattern with raw SQL queries
  - Database file: `tp6.dbtp6.db` stored in `FileSystem.AppDataDirectory`
- `Repositories/`: Repository pattern implementations
  - `NoteRepository`: Implements `INoteRepository`, wraps NoteDAO with async interface
- `WebServices/`: Remote API service implementations
  - `NotesRemoteService`: Implements `INoteRemoteService` using HttpClient
  - `UserProfileRemoteService`: Implements `IUserProfileRemoteService` using HttpClient
- `Sync/`: Synchronization services
  - `NotesSyncService`: Implements `ISyncService`, orchestrates pull/push operations
- `DependencyInjection.cs`: Extension method `AddInfrastructure()` for IoC registration

**Services Layer** (`Services/Persistence/`)
- Interface definitions: `IDaoNote`, `IDaoUser`
- Contracts for data access operations (CRUD operations)

**ViewModels Layer** (`ViewModels/`)
- MVVM pattern using CommunityToolkit.Mvvm
- `BaseViewModel`: Inherits from `ObservableObject`
- `MainViewModel`: Manages note list with `ObservableCollection<Note>`
- `CreateNoteViewModel`: Handles note creation
- `ProfileViewModel`: User profile management
- Uses `[RelayCommand]` attributes for command generation
- Can inject Application interfaces (INoteRepository, ISyncService, etc.) via DI

**Views Layer** (`Views/`)
- XAML pages: `MainPage`, `CreateNotePage`, `ProfilePage`
- Shell-based navigation configured in `AppShell.xaml`

**Converters** (`Converters/`)
- `DateTimeToStringConverter`: Format datetime values for UI
- `InverseBooleanConverter`: Boolean inversion for bindings

### Key Architectural Decisions

1. **Clean Architecture**: Application layer defines interfaces; Infrastructure implements them
2. **Singleton Pattern**: `DaoContext`, `NoteService`, and `UserService` use singleton instances accessed via `Instance` property
3. **Repository Pattern**: `NoteRepository` abstracts data access and provides async interface over synchronous DAOs
4. **DAO Pattern**: Raw SQL queries in DAO classes instead of ORM for explicit control
5. **Dependency Injection**: All services registered via IoC in `MauiProgram.cs`:
   - Infrastructure: `builder.Services.AddInfrastructure()` - registers repositories, web services, sync service
   - Legacy: `NotesApiService` as Singleton
   - ViewModels as Transient
   - HttpClient configured via `AddHttpClient<TInterface, TImplementation>()` with base URL
6. **HttpClientFactory**: Typed clients for web services (not direct HttpClient instantiation)
7. **Shell Navigation**: Routes registered in `AppShell.xaml.cs` constructor with try-catch to handle re-registration
8. **Async/Await**: All Infrastructure operations are asynchronous with CancellationToken support

## Database Schema

The SQLite database is initialized in `DaoContext.InitializeDatabase()`:

**Note Table**
- Id (TEXT, PRIMARY KEY)
- Title (TEXT, NOT NULL)
- Content (TEXT, NOT NULL)
- CreatedAt (TEXT, NOT NULL) - stored as ISO 8601 string
- IsSynced (INTEGER, NOT NULL, DEFAULT 0) - tracks if note has been synced with remote
- PendingSync (INTEGER, NOT NULL, DEFAULT 0) - marks notes pending upload to remote

**User Table**
- Id (INTEGER, PRIMARY KEY, AUTOINCREMENT)
- FirstName (TEXT, NOT NULL)
- LastName (TEXT, NOT NULL)
- MemberSince (TEXT, NOT NULL) - ISO 8601 format
- NotificationsEnabled (INTEGER, DEFAULT 0)
- DarkModeEnabled (INTEGER, DEFAULT 1)
- ProfileImagePath (TEXT)

## Dependencies

Key NuGet packages (from TP6.csproj):
- `CommunityToolkit.Mvvm` (8.4.0): MVVM helpers and source generators
- `sqlite-net-pcl` (1.9.172): SQLite database access
- `Microsoft.Maui.Controls`: MAUI framework

## Navigation Structure

The app uses Shell-based tab navigation:
- Tab 1: "Accueil" (main) - MainPage (note list)
- Tab 2: "Creer" (create) - CreateNotePage (create new note)
- Modal: "profile" route - ProfilePage (user profile)

Routes are registered programmatically in `AppShell.xaml.cs` and navigation uses `Shell.Current.GoToAsync()`.

## Important Implementation Details

1. **Default Notes**: `NoteService` automatically creates 3 default notes on first run if database is empty
2. **DateTime Storage**: All DateTime values are stored as ISO 8601 strings using `.ToString("o")` format
3. **Boolean in SQLite**: Stored as INTEGER (0/1) and converted in DAO layer
4. **API Service**: Uses JSONPlaceholder (`https://jsonplaceholder.typicode.com`) for demo purposes
   - **Important**: JSONPlaceholder does NOT persist write operations (POST/PUT/DELETE)
   - Only GET operations return real data
   - Write operations return success but data is not stored server-side
   - This is simulated/mock behavior for testing
5. **Sync Behavior**:
   - Pull: Downloads all posts from JSONPlaceholder and stores locally as notes
   - Push: Uploads pending local notes (simulated, not actually persisted on server)
   - Notes track sync status via `IsSynced` and `PendingSync` fields
6. **ViewModel Refresh**: `MainViewModel.RefreshNotes()` method should be called after creating notes to update the UI
7. **Logging**: All Infrastructure services use `ILogger<T>` for comprehensive logging
8. **Error Handling**: Infrastructure services catch exceptions and return safe defaults (null/empty) rather than throwing

## Web Service Integration

See `Infrastructure/README.md` for detailed documentation on:
- Using `ISyncService` to sync notes between local and remote
- Using `INoteRemoteService` for direct API calls
- Using `IUserProfileRemoteService` to fetch user profiles
- Using `INoteRepository` for local async data access
- Code examples for ViewModels
- Testing the sync functionality
