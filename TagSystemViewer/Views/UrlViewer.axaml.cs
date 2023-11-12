using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TagSystemViewer.ViewModels;

namespace TagSystemViewer.Views;

public partial class UrlViewer : Window
{
    public UrlViewer()
    {
        InitializeComponent();
        DataContext = new UrlViewerViewModel();

    }

    private void OpenTagWindow(object? sender, RoutedEventArgs e)
    {
        var window = new MainWindow();
        window.Show();
    }

    private void OpenUrlWindow(object? sender, RoutedEventArgs e)
    {
        var window = new URLGridEditor();
        window.Show();
    }
}