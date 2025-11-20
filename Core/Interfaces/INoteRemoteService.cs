using TP6.Models.Entity;

namespace TP6.Core.Interfaces;

/// <summary>
/// Remote service interface for Note API operations using JSONPlaceholder
/// Note: JSONPlaceholder does not persist write changes; POST/PUT/DELETE return simulated success
/// </summary>
public interface INoteRemoteService
{
    /// <summary>
    /// Gets all notes from remote API (maps to GET /posts)
    /// </summary>
    Task<IReadOnlyList<Note>> GetNotesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single note by ID from remote API (maps to GET /posts/{id})
    /// </summary>
    /// <param name="id">The note identifier (maps to post ID)</param>
    Task<Note?> GetNoteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new note on remote API (maps to POST /posts)
    /// Note: JSONPlaceholder simulates success but does not persist the data
    /// </summary>
    Task<Note?> CreateNoteAsync(Note note, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing note on remote API (maps to PUT /posts/{id})
    /// Note: JSONPlaceholder simulates success but does not persist changes
    /// </summary>
    Task<Note?> UpdateNoteAsync(Note note, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a note from remote API (maps to DELETE /posts/{id})
    /// Note: JSONPlaceholder simulates success but does not actually delete
    /// </summary>
    /// <param name="id">The note identifier to delete</param>
    Task<bool> DeleteNoteAsync(int id, CancellationToken cancellationToken = default);
}
