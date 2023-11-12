using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TagSystemViewer.ViewModels;

namespace TagSystemViewer.Views;

public partial class URLGridEditor : Window
{
    public URLGridEditor()
    {
        InitializeComponent();
        DataContext = new UrlGridEditorViewModel();
    }

    private void OpenTagWindow(object? sender, RoutedEventArgs e)
    {
        var tagsWindow = new TagGridEditor();
        tagsWindow.Show(this);
    }
}