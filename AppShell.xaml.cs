namespace TP6;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Register routes here so Shell navigation works with our pages
		try
		{
			Routing.RegisterRoute("profile", typeof(Views.ProfilePage));
			Routing.RegisterRoute("create", typeof(Views.CreateNotePage));
			Routing.RegisterRoute("main", typeof(Views.MainPage));
		}
		catch
		{
			// ignore if already registered
		}
	}
}


	