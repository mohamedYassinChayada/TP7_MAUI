using TP6.ViewModels;

namespace TP6.Views;

public partial class CreateNotePage : ContentPage
{
    public CreateNotePage()
    {
        InitializeComponent();
        BindingContext = new CreateNoteViewModel();
    }
}
