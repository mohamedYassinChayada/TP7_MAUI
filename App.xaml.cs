using TP6.Services;

namespace TP6;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// Initialize and apply theme based on user preference
		ThemeService.Instance.ApplyTheme();

		MainPage = new AppShell();
	}
}

