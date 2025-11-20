using Microsoft.Extensions.Logging;
using TP6.Core.Interfaces;
using TP6.Infrastructure.Persistance;
using TP6.Models.Entity;
using TP6.Services.Persistence;

namespace TP6.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for local Note data access using SQLite
/// </summary>
public class NoteRepository : INoteRepository
{
    private readonly IDaoNote _noteDao;
    private readonly ILogger<NoteRepository> _logger;

    public NoteRepository(IDaoNote noteDao, ILogger<NoteRepository> logger)
    {
        _noteDao = noteDao;
        _logger = logger;
    }

    public Task<IReadOnlyList<Note>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var notes = _noteDao.GetAll();
            return Task.FromResult<IReadOnlyList<Note>>(notes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all notes from local storage");
            return Task.FromResult<IReadOnlyList<Note>>(new List<Note>());
        }
    }

    public Task<Note?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var note = _noteDao.GetById(id);
            return Task.FromResult(note);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting note {NoteId} from local storage", id);
            return Task.FromResult<Note?>(null);
        }
    }

    public Task<IReadOnlyList<Note>> GetPendingSyncAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var allNotes = _noteDao.GetAll();
            var pendingNotes = allNotes.Where(n => n.PendingSync).ToList();
            return Task.FromResult<IReadOnlyList<Note>>(pendingNotes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending sync notes from local storage");
            return Task.FromResult<IReadOnlyList<Note>>(new List<Note>());
        }
    }

    public Task<int> UpsertAsync(Note note, CancellationToken cancellationToken = default)
    {
        try
        {
            var existing = _noteDao.GetById(note.Id);
            if (existing != null)
            {
                return UpdateAsync(note, cancellationToken);
            }
            else
            {
                return InsertAsync(note, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error upserting note {NoteId} to local storage", note.Id);
            return Task.FromResult(0);
        }
    }

    public Task<int> InsertAsync(Note note, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = _noteDao.Insert(note);
            _logger.LogInformation("Inserted note {NoteId} to local storage", note.Id);
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting note {NoteId} to local storage", note.Id);
            return Task.FromResult(0);
        }
    }

    public Task<int> UpdateAsync(Note note, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = _noteDao.Update(note);
            _logger.LogInformation("Updated note {NoteId} in local storage", note.Id);
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating note {NoteId} in local storage", note.Id);
            return Task.FromResult(0);
        }
    }

    public Task<int> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = _noteDao.Delete(id);
            _logger.LogInformation("Deleted note {NoteId} from local storage", id);
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting note {NoteId} from local storage", id);
            return Task.FromResult(0);
        }
    }

    public Task<int> DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = _noteDao.DeleteAll();
            _logger.LogInformation("Deleted all notes from local storage");
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting all notes from local storage");
            return Task.FromResult(0);
        }
    }
}
