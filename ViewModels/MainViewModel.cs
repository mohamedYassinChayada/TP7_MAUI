using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TP6.Core.Interfaces;
using TP6.Models.Business;
using TP6.Models.Entity;

namespace TP6.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly NoteService _noteService;
    private readonly ISyncService? _syncService;
    private readonly INoteRepository? _noteRepository;

    [ObservableProperty]
    private bool isSyncing = false;

    [ObservableProperty]
    private string syncStatus = string.Empty;

    public ObservableCollection<Note> Notes { get; } = new();

    // Constructor for DI (when ISyncService is available)
    public MainViewModel(ISyncService syncService, INoteRepository noteRepository)
    {
        _noteService = NoteService.Instance;
        _syncService = syncService;
        _noteRepository = noteRepository;
        LoadNotes();
    }

    // Fallback constructor (for backward compatibility)
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

    [RelayCommand]
    private async Task SyncNotes()
    {
        if (_syncService == null || _noteRepository == null)
        {
            await Shell.Current.DisplayAlert("Erreur", "Le service de synchronisation n'est pas disponible", "OK");
            return;
        }

        if (IsSyncing) return;

        try
        {
            IsSyncing = true;
            SyncStatus = "Synchronisation en cours...";

            // Pull notes from JSONPlaceholder
            await _syncService.PullNotesAsync();

            // Reload notes from local database
            await RefreshNotesAsync();

            SyncStatus = $"✓ {Notes.Count} notes synchronisées";
            await Shell.Current.DisplayAlert("Succès", $"{Notes.Count} notes ont été importées depuis JSONPlaceholder!", "OK");
        }
        catch (Exception ex)
        {
            SyncStatus = "✗ Erreur de synchronisation";
            await Shell.Current.DisplayAlert("Erreur", $"Erreur lors de la synchronisation: {ex.Message}", "OK");
        }
        finally
        {
            IsSyncing = false;
            // Clear status after 3 seconds
            await Task.Delay(3000);
            SyncStatus = string.Empty;
        }
    }

    private async Task RefreshNotesAsync()
    {
        if (_noteRepository != null)
        {
            var notes = await _noteRepository.GetAllAsync();
            Notes.Clear();
            foreach (var note in notes)
            {
                Notes.Add(note);
            }
        }
        else
        {
            LoadNotes();
        }
    }

    public void RefreshNotes()
    {
        LoadNotes();
    }
}
