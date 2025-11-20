using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TP6.Models.Business;
using TP6.Models.Entity;

namespace TP6.ViewModels;

public partial class CreateNoteViewModel : BaseViewModel
{
    private readonly NoteService _noteService;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string content = string.Empty;

    public CreateNoteViewModel()
    {
        _noteService = NoteService.Instance;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Content))
        {
            await Shell.Current.DisplayAlert("Info", "Note vide", "OK");
            return;
        }

        var note = new Note { Title = Title, Content = Content };
        _noteService.addnote(note);

        Title = string.Empty;
        Content = string.Empty;

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
