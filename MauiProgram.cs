using Microsoft.Extensions.Logging;

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

		builder.Services.AddSingleton<TP6.Models.Business.NotesApiService>();
		builder.Services.AddTransient<TP6.ViewModels.MainViewModel>();
		builder.Services.AddTransient<TP6.ViewModels.ProfileViewModel>();

	#if DEBUG
		builder.Logging.AddDebug();
	#endif

		return builder.Build();
	}
}

