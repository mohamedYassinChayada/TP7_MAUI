using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using TP6.Models.Business;
using TP6.Models.Entity;

namespace TP6.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly NoteService _noteService;

    public ObservableCollection<Note> Notes { get; } = new();

    public MainViewModel()
    {
        _noteService = NoteService.Instance;
        LoadNotes();
    }

    private void LoadNotes()
    {
        Notes.Clear();
        foreach (var note in _noteService.getnotes())
        {
            Notes.Add(note);
        }
    }

    [RelayCommand]
    private async Task NavigateToCreateNote()
    {
        await Shell.Current.GoToAsync("create");
    }

    [RelayCommand]
    private async Task NavigateToProfile()
    {
        await Shell.Current.GoToAsync("profile");
    }

    public void RefreshNotes()
    {
        LoadNotes();
    }
}
