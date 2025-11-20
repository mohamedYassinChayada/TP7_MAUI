using TP6.ViewModels;

namespace TP6.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
		BindingContext = new MainViewModel();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is MainViewModel vm)
		{
			vm.RefreshNotes();
		}
	}
}


