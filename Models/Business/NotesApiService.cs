using System.Net.Http;
using System.Text;
using System.Text.Json;
using TP6.Models.Entity;

namespace TP6.Models.Business;

public class NotesApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://jsonplaceholder.typicode.com";

    public NotesApiService()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }


    public async Task<List<Note>> GetNotesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/posts");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Note>>(json);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
        }
        return new List<Note>();
    }


    public async Task<Note?> CreateNoteAsync(Note note)
    {
        try
        {
            var json = JsonSerializer.Serialize(note);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/posts", content);
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Note>(responseJson);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error creating: {ex.Message}");
        }
        return null;
    }

    public async Task<User?> GetUserAsync(int userId = 1)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/users/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<User>(json);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"User error: {ex.Message}");
        }
        return null;
    }
}
