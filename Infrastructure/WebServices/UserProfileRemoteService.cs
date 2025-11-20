using System.Text.Json;
using Microsoft.Extensions.Logging;
using TP6.Core.Interfaces;
using TP6.Models.Entity;

namespace TP6.Infrastructure.WebServices;

/// <summary>
/// Remote service implementation for User Profile API operations using JSONPlaceholder
/// </summary>
public class UserProfileRemoteService : IUserProfileRemoteService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserProfileRemoteService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserProfileRemoteService(HttpClient httpClient, ILogger<UserProfileRemoteService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Gets user profile data from remote API (maps to GET /users/{id})
    /// JSONPlaceholder user data is mapped to User entity
    /// </summary>
    public async Task<User?> GetUserAsync(int id = 1, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching user profile {UserId} from remote API", id);
            var response = await _httpClient.GetAsync($"/users/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var jsonUser = JsonSerializer.Deserialize<JsonPlaceholderUser>(json, _jsonOptions);

                if (jsonUser != null)
                {
                    var user = new User
                    {
                        Id = jsonUser.Id,
                        FirstName = jsonUser.Name?.Split(' ').FirstOrDefault() ?? string.Empty,
                        LastName = jsonUser.Name?.Split(' ').Skip(1).FirstOrDefault() ?? string.Empty,
                        MemberSince = DateTime.Now.AddMonths(-6), // Simulated member since date
                        NotificationsEnabled = false,
                        DarkModeEnabled = true,
                        ProfileImagePath = string.Empty
                    };

                    _logger.LogInformation("Successfully fetched user profile {UserId} from remote API", id);
                    return user;
                }
            }
            else
            {
                _logger.LogWarning("Failed to fetch user profile {UserId} from remote API. Status: {StatusCode}", id, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user profile {UserId} from remote API", id);
        }

        return null;
    }

    /// <summary>
    /// Internal class to map JSONPlaceholder user structure
    /// </summary>
    private class JsonPlaceholderUser
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
    }
}
