using Microsoft.Extensions.Logging;
using TP6.Infrastructure;

namespace TP6;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
		    .UseMauiApp<App>()
		    .ConfigureFonts(fonts =>
		    {
			fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
		    });

		// Register Infrastructure services (repositories, web services, sync)
		builder.Services.AddInfrastructure();

		// Register legacy services
		builder.Services.AddSingleton<TP6.Models.Business.NotesApiService>();

		// Register ViewModels
		builder.Services.AddTransient<TP6.ViewModels.MainViewModel>();
		builder.Services.AddTransient<TP6.ViewModels.ProfileViewModel>();
		builder.Services.AddTransient<TP6.ViewModels.CreateNoteViewModel>();

		// Register Pages
		builder.Services.AddTransient<TP6.Views.MainPage>();
		builder.Services.AddTransient<TP6.Views.ProfilePage>();
		builder.Services.AddTransient<TP6.Views.CreateNotePage>();

	#if DEBUG
		builder.Logging.AddDebug();
	#endif

		return builder.Build();
	}
}

