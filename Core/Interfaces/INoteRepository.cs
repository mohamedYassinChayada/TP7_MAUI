using TP6.Models.Entity;

namespace TP6.Core.Interfaces;

/// <summary>
/// Repository interface for local Note data access
/// </summary>
public interface INoteRepository
{
    /// <summary>
    /// Gets all notes from local storage, ordered by creation date descending
    /// </summary>
    Task<IReadOnlyList<Note>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a note by its unique identifier
    /// </summary>
    Task<Note?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets notes that are pending synchronization with remote service
    /// </summary>
    Task<IReadOnlyList<Note>> GetPendingSyncAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a new note or updates existing note (upsert operation)
    /// </summary>
    Task<int> UpsertAsync(Note note, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a new note into local storage
    /// </summary>
    Task<int> InsertAsync(Note note, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing note in local storage
    /// </summary>
    Task<int> UpdateAsync(Note note, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a note by its identifier
    /// </summary>
    Task<int> DeleteAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all notes from local storage
    /// </summary>
    Task<int> DeleteAllAsync(CancellationToken cancellationToken = default);
}
