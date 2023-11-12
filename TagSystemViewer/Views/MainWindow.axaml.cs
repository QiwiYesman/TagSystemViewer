using Avalonia.Controls;
using TagSystemViewer.ViewModels;

namespace TagSystemViewer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
    
}