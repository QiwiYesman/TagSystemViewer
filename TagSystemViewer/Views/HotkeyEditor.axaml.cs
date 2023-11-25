using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace TagSystemViewer.Views;

public partial class HotkeyEditor : Window
{
    public HotkeyEditor()
    {
        InitializeComponent();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (!KeyGesturesText.IsEnabled) return;
        KeyGesturesText.Text = e.Key.ToString();
    }
}