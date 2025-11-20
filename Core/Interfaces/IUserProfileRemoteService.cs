using TP6.Models.Entity;

namespace TP6.Core.Interfaces;

/// <summary>
/// Remote service interface for User Profile API operations using JSONPlaceholder
/// </summary>
public interface IUserProfileRemoteService
{
    /// <summary>
    /// Gets user profile data from remote API (maps to GET /users/{id})
    /// </summary>
    /// <param name="id">The user identifier (default: 1)</param>
    Task<User?> GetUserAsync(int id = 1, CancellationToken cancellationToken = default);
}
