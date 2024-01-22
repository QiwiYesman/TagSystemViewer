using Avalonia.Controls;
using Avalonia.Interactivity;
using TagSystemViewer.ViewModels;

namespace TagSystemViewer.Views;

public partial class UrlViewer : Window
{
    public UrlViewer()
    {
        InitializeComponent();
    }

    private void OpenTagWindow(object? sender, RoutedEventArgs e)
    {
        var window = new TagGridEditor();
        window.Show(this);
    }

    private void OpenUrlWindow(object? sender, RoutedEventArgs e)
    {
        var window = new UrlEditor();
        window.Show(this);
    }

    private void OpenDatabaseWindow(object? sender, RoutedEventArgs e)
    {
        var window = new DatabaseConfigEditor();
        window.Show(this);
    }

    private void OpenHotkeyWindow(object? sender, RoutedEventArgs e)
    {
        var window = new HotkeyEditor() { DataContext = new HotkeyEditorViewModel() };
        window.Show(this);
    }
}