using Microsoft.Extensions.DependencyInjection;
using TP6.Core.Interfaces;
using TP6.Infrastructure.Persistance;
using TP6.Infrastructure.Repositories;
using TP6.Infrastructure.Sync;
using TP6.Infrastructure.WebServices;
using TP6.Services.Persistence;

namespace TP6.Infrastructure;

/// <summary>
/// Extension class for registering Infrastructure layer dependencies
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Infrastructure services including repositories, web services, and sync services
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register local DAOs as Singletons (they use singleton DaoContext internally)
        services.AddSingleton<IDaoNote, NoteDAO>();
        services.AddSingleton<IDaoUser, UserDAO>();

        // Register Repository with async interface
        services.AddSingleton<INoteRepository, NoteRepository>();

        // Register HttpClient for remote services with base address
        services.AddHttpClient<INoteRemoteService, NotesRemoteService>(client =>
        {
            client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient<IUserProfileRemoteService, UserProfileRemoteService>(client =>
        {
            client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Register Sync Service as Singleton
        services.AddSingleton<ISyncService, NotesSyncService>();

        return services;
    }
}
