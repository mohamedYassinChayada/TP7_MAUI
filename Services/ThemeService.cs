using TP6.Models.Business;

namespace TP6.Services;

/// <summary>
/// Service to manage application-wide theme (Light/Dark mode)
/// </summary>
public class ThemeService
{
    private static ThemeService? _instance;
    public static ThemeService Instance => _instance ??= new ThemeService();

    private readonly UserService _userService;

    private ThemeService()
    {
        _userService = UserService.Instance;
        ApplyTheme();
    }

    /// <summary>
    /// Apply theme based on user's dark mode preference
    /// </summary>
    public void ApplyTheme()
    {
        var user = _userService.GetCurrentUser();
        var isDarkMode = user?.DarkModeEnabled ?? true;

        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;
        }
    }

    /// <summary>
    /// Toggle between light and dark mode
    /// </summary>
    public void ToggleTheme()
    {
        var user = _userService.GetCurrentUser();
        var newDarkMode = !user.DarkModeEnabled;
        _userService.UpdateUserSettings(user.NotificationsEnabled, newDarkMode);
        ApplyTheme();
    }

    /// <summary>
    /// Get current dark mode state
    /// </summary>
    public bool IsDarkMode()
    {
        var user = _userService.GetCurrentUser();
        return user?.DarkModeEnabled ?? true;
    }
}
