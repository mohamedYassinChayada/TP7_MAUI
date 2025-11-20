using TP6.Services;
using TP6.ViewModels;

namespace TP6.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = new ProfileViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ProfileViewModel vm)
        {
            vm.RefreshStats();
        }
    }

    private void OnNotificationsToggled(object sender, ToggledEventArgs e)
    {
        if (BindingContext is ProfileViewModel vm)
        {
            vm.ToggleNotificationsCommand.Execute(null);
        }
    }

    private void OnDarkModeToggled(object sender, ToggledEventArgs e)
    {
        if (BindingContext is ProfileViewModel vm)
        {
            vm.ToggleDarkModeCommand.Execute(null);
            // Apply theme immediately
            ThemeService.Instance.ApplyTheme();
        }
    }
}
