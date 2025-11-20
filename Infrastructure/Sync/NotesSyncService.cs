using Microsoft.Extensions.Logging;
using TP6.Core.Interfaces;

namespace TP6.Infrastructure.Sync;

/// <summary>
/// Service for synchronizing notes between local storage and remote API
/// Orchestrates pull (remote to local) and push (local to remote) operations
/// </summary>
public class NotesSyncService : ISyncService
{
    private readonly INoteRemoteService _remoteService;
    private readonly INoteRepository _localRepository;
    private readonly ILogger<NotesSyncService> _logger;

    public NotesSyncService(
        INoteRemoteService remoteService,
        INoteRepository localRepository,
        ILogger<NotesSyncService> logger)
    {
        _remoteService = remoteService;
        _localRepository = localRepository;
        _logger = logger;
    }

    /// <summary>
    /// Synchronizes notes between local and remote storage
    /// - Pull: Fetches all remote notes and upserts them into local storage
    /// - Push: Uploads pending local notes to remote API and marks them as synced
    /// </summary>
    public async Task SyncNotesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting notes synchronization");

            // Pull: Get remote notes and store locally
            await PullNotesAsync(cancellationToken);

            // Push: Upload pending local notes to remote
            await PushNotesAsync(cancellationToken);

            _logger.LogInformation("Notes synchronization completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during notes synchronization");
        }
    }

    /// <summary>
    /// Pulls all notes from remote API and stores them locally
    /// Remote notes are marked as synced and not pending
    /// </summary>
    public async Task PullNotesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Pulling notes from remote API");

            var remoteNotes = await _remoteService.GetNotesAsync(cancellationToken);

            if (remoteNotes != null && remoteNotes.Count > 0)
            {
                _logger.LogInformation("Received {Count} notes from remote API", remoteNotes.Count);

                foreach (var note in remoteNotes)
                {
                    // Ensure notes from remote are marked as synced
                    note.IsSynced = true;
                    note.PendingSync = false;

                    await _localRepository.UpsertAsync(note, cancellationToken);
                }

                _logger.LogInformation("Successfully pulled and stored {Count} notes locally", remoteNotes.Count);
            }
            else
            {
                _logger.LogWarning("No notes received from remote API during pull");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pulling notes from remote API");
            throw;
        }
    }

    /// <summary>
    /// Pushes pending local notes to remote API
    /// Note: JSONPlaceholder does not persist changes, so this simulates the push operation
    /// After successful push (simulated), notes are marked as synced locally
    /// </summary>
    public async Task PushNotesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Pushing pending local notes to remote API");

            var pendingNotes = await _localRepository.GetPendingSyncAsync(cancellationToken);

            if (pendingNotes != null && pendingNotes.Count > 0)
            {
                _logger.LogInformation("Found {Count} pending notes to push", pendingNotes.Count);

                foreach (var note in pendingNotes)
                {
                    try
                    {
                        // Determine if this is a create or update based on whether it has been synced before
                        var result = note.IsSynced
                            ? await _remoteService.UpdateNoteAsync(note, cancellationToken)
                            : await _remoteService.CreateNoteAsync(note, cancellationToken);

                        if (result != null)
                        {
                            // Mark as synced and no longer pending
                            note.IsSynced = true;
                            note.PendingSync = false;

                            // Update the local note with sync status
                            await _localRepository.UpsertAsync(note, cancellationToken);

                            _logger.LogInformation("Successfully pushed note {NoteId} to remote API (simulated)", note.Id);
                        }
                        else
                        {
                            _logger.LogWarning("Failed to push note {NoteId} to remote API", note.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error pushing note {NoteId} to remote API", note.Id);
                        // Continue with next note even if one fails
                    }
                }

                _logger.LogInformation("Completed pushing pending notes to remote API");
            }
            else
            {
                _logger.LogInformation("No pending notes to push");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pushing notes to remote API");
            throw;
        }
    }
}
