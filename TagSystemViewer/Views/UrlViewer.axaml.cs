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

    private void OpenWindow<T>(object? dataContext) where T: Window, new()
    {
        T window = new T();
        window.Show(this);
        if (dataContext is null) return;
        window.DataContext = dataContext;
    }
    private void OpenTagWindow(object? sender, RoutedEventArgs e)
    {
        OpenWindow<TagGridEditor>(null);
    }

    private void OpenUrlWindow(object? sender, RoutedEventArgs e)
    {
        OpenWindow<UrlEditor>(null);
    }

    private void OpenDatabaseWindow(object? sender, RoutedEventArgs e)
    {
        OpenWindow<DatabaseConfigEditor>(null);
    }

    private void OpenHotkeyWindow(object? sender, RoutedEventArgs e)
    {
        OpenWindow<HotkeyEditor>(new HotkeyEditorViewModel());
    }
}