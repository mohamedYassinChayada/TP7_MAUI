namespace TP6.Core.Interfaces;

/// <summary>
/// Service interface for synchronizing notes between local storage and remote API
/// </summary>
public interface ISyncService
{
    /// <summary>
    /// Synchronizes notes between local and remote storage
    /// - Pull: Fetches all remote notes and upserts them into local storage
    /// - Push (optional): Uploads pending local notes to remote API and marks them as synced
    /// </summary>
    Task SyncNotesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Pulls all notes from remote API and stores them locally
    /// </summary>
    Task PullNotesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Pushes pending local notes to remote API
    /// Note: JSONPlaceholder does not persist changes, so this simulates the push operation
    /// </summary>
    Task PushNotesAsync(CancellationToken cancellationToken = default);
}
