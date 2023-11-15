using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TagSystemViewer.Models;
using TagSystemViewer.ViewModels;

namespace TagSystemViewer.Views;

public partial class DatabaseConfigEditor : Window
{
    public DatabaseConfigEditor()
    {
        InitializeComponent();
        DataContext = new DatabaseConfigViewModel();
    }
}