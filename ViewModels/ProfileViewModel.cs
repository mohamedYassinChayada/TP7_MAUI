using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TP6.Models.Business;
using TP6.Models.Entity;

namespace TP6.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly NoteService _noteService;
    private readonly UserService _userService;

    [ObservableProperty]
    private User currentUser;

    [ObservableProperty]
    private bool isEditing = false;

    [ObservableProperty]
    private string editFirstName = string.Empty;

    [ObservableProperty]
    private string editLastName = string.Empty;

    public int NotesCount => _noteService.count();

    public string MemberSinceDays => $"• Membre depuis {CurrentUser.DaysSinceMember} jours";

    public ProfileViewModel()
    {
        _noteService = NoteService.Instance;
        _userService = UserService.Instance;
        currentUser = _userService.GetCurrentUser();
        EditLastName = CurrentUser.LastName;
    }

    [RelayCommand]
    private void StartEdit()
    {
        IsEditing = true;
        EditFirstName = CurrentUser.FirstName;
        EditLastName = CurrentUser.LastName;
    }

    [RelayCommand]
    private void SaveProfile()
    {
        _userService.UpdateUserProfile(EditFirstName, EditLastName);
        CurrentUser = _userService.GetCurrentUser();
        IsEditing = false;
        OnPropertyChanged(nameof(MemberSinceDays));
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
        EditFirstName = CurrentUser.FirstName;
        EditLastName = CurrentUser.LastName;
    }

    [RelayCommand]
    private void ToggleNotifications()
    {
        _userService.UpdateUserSettings(!CurrentUser.NotificationsEnabled, CurrentUser.DarkModeEnabled);
        CurrentUser = _userService.GetCurrentUser();
    }

    [RelayCommand]
    private void ToggleDarkMode()
    {
        _userService.UpdateUserSettings(CurrentUser.NotificationsEnabled, !CurrentUser.DarkModeEnabled);
        CurrentUser = _userService.GetCurrentUser();
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    public void RefreshStats()
    {
        OnPropertyChanged(nameof(NotesCount));
        OnPropertyChanged(nameof(MemberSinceDays));
        CurrentUser = _userService.GetCurrentUser();
    }
}
