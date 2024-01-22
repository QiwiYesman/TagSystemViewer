using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TagSystemViewer.ViewModels;

namespace TagSystemViewer.Views;

public partial class UrlEditor : Window
{
    public UrlEditor()
    {
        InitializeComponent();
        DataContext = new UrlEditorViewModel();
    }

    private void OpenTagWindow(object? sender, RoutedEventArgs e)
    {
        var tagsWindow = new TagGridEditor();
        tagsWindow.Show(this);
    }
}