using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TP6.Core.Interfaces;
using TP6.Models.Entity;

namespace TP6.Infrastructure.WebServices;

/// <summary>
/// Remote service implementation for Note API operations using JSONPlaceholder
/// Note: JSONPlaceholder does not persist write changes; POST/PUT/DELETE return simulated success
/// </summary>
public class NotesRemoteService : INoteRemoteService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotesRemoteService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public NotesRemoteService(HttpClient httpClient, ILogger<NotesRemoteService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Gets all notes from remote API (maps to GET /posts)
    /// JSONPlaceholder posts are mapped to Note entities
    /// </summary>
    public async Task<IReadOnlyList<Note>> GetNotesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching all notes from remote API");
            var response = await _httpClient.GetAsync("/posts", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var posts = JsonSerializer.Deserialize<List<JsonPlaceholderPost>>(json, _jsonOptions);

                if (posts != null)
                {
                    var notes = posts.Select(p => new Note
                    {
                        Id = p.Id.ToString(),
                        Title = p.Title ?? string.Empty,
                        Content = p.Body ?? string.Empty,
                        CreatedAt = DateTime.Now,
                        IsSynced = true,
                        PendingSync = false
                    }).ToList();

                    _logger.LogInformation("Successfully fetched {Count} notes from remote API", notes.Count);
                    return notes;
                }
            }
            else
            {
                _logger.LogWarning("Failed to fetch notes from remote API. Status: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching notes from remote API");
        }

        return new List<Note>();
    }

    /// <summary>
    /// Gets a single note by ID from remote API (maps to GET /posts/{id})
    /// </summary>
    public async Task<Note?> GetNoteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching note {NoteId} from remote API", id);
            var response = await _httpClient.GetAsync($"/posts/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var post = JsonSerializer.Deserialize<JsonPlaceholderPost>(json, _jsonOptions);

                if (post != null)
                {
                    var note = new Note
                    {
                        Id = post.Id.ToString(),
                        Title = post.Title ?? string.Empty,
                        Content = post.Body ?? string.Empty,
                        CreatedAt = DateTime.Now,
                        IsSynced = true,
                        PendingSync = false
                    };

                    _logger.LogInformation("Successfully fetched note {NoteId} from remote API", id);
                    return note;
                }
            }
            else
            {
                _logger.LogWarning("Failed to fetch note {NoteId} from remote API. Status: {StatusCode}", id, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching note {NoteId} from remote API", id);
        }

        return null;
    }

    /// <summary>
    /// Creates a new note on remote API (maps to POST /posts)
    /// Note: JSONPlaceholder simulates success but does not persist the data
    /// </summary>
    public async Task<Note?> CreateNoteAsync(Note note, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating note on remote API");

            var post = new JsonPlaceholderPost
            {
                Title = note.Title,
                Body = note.Content,
                UserId = 1
            };

            var json = JsonSerializer.Serialize(post, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/posts", content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                var createdPost = JsonSerializer.Deserialize<JsonPlaceholderPost>(responseJson, _jsonOptions);

                if (createdPost != null)
                {
                    note.Id = createdPost.Id.ToString();
                    note.IsSynced = true;
                    note.PendingSync = false;

                    _logger.LogInformation("Successfully created note on remote API (simulated). ID: {NoteId}", note.Id);
                    return note;
                }
            }
            else
            {
                _logger.LogWarning("Failed to create note on remote API. Status: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating note on remote API");
        }

        return null;
    }

    /// <summary>
    /// Updates an existing note on remote API (maps to PUT /posts/{id})
    /// Note: JSONPlaceholder simulates success but does not persist changes
    /// </summary>
    public async Task<Note?> UpdateNoteAsync(Note note, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating note {NoteId} on remote API", note.Id);

            var post = new JsonPlaceholderPost
            {
                Id = int.TryParse(note.Id, out var id) ? id : 1,
                Title = note.Title,
                Body = note.Content,
                UserId = 1
            };

            var json = JsonSerializer.Serialize(post, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/posts/{post.Id}", content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                note.IsSynced = true;
                note.PendingSync = false;

                _logger.LogInformation("Successfully updated note {NoteId} on remote API (simulated)", note.Id);
                return note;
            }
            else
            {
                _logger.LogWarning("Failed to update note {NoteId} on remote API. Status: {StatusCode}", note.Id, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating note {NoteId} on remote API", note.Id);
        }

        return null;
    }

    /// <summary>
    /// Deletes a note from remote API (maps to DELETE /posts/{id})
    /// Note: JSONPlaceholder simulates success but does not actually delete
    /// </summary>
    public async Task<bool> DeleteNoteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting note {NoteId} from remote API", id);
            var response = await _httpClient.DeleteAsync($"/posts/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully deleted note {NoteId} from remote API (simulated)", id);
                return true;
            }
            else
            {
                _logger.LogWarning("Failed to delete note {NoteId} from remote API. Status: {StatusCode}", id, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting note {NoteId} from remote API", id);
        }

        return false;
    }

    /// <summary>
    /// Internal class to map JSONPlaceholder post structure
    /// </summary>
    private class JsonPlaceholderPost
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
    }
}
