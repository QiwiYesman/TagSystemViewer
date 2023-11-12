using Avalonia.Controls;
using TagSystemViewer.ViewModels;

namespace TagSystemViewer.Views;

public partial class TagGridEditor : Window
{
    public TagGridEditor()
    {
        InitializeComponent();
        DataContext = new TagEditorViewModel();
    }
    
}