using TP6.ViewModels;

namespace TP6.Views;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
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


