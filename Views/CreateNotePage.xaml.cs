using TP6.ViewModels;

namespace TP6.Views;

public partial class CreateNotePage : ContentPage
{
    public CreateNotePage(CreateNoteViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
