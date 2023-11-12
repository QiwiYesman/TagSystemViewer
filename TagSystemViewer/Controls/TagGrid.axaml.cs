using System.Collections;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using TagSystemViewer.Models;

namespace TagSystemViewer.Controls;

public class TagGrid : TemplatedControl
{
    private IEnumerable _itemsSource;
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<TagGrid, string>("Text");
    
    public static readonly DirectProperty<TagGrid, IEnumerable> ItemsSourceProperty =
        AvaloniaProperty.RegisterDirect<TagGrid, IEnumerable>("ItemsSource",
            o => o.ItemsSource,
            (o, v) => o.ItemsSource = v);
    public static readonly StyledProperty<Tag> SelectedItemProperty =
        AvaloniaProperty.Register<TagGrid, Tag>("SelectedItem");
    public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<TagGrid, ICommand>("Command");
    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public IEnumerable ItemsSource
    {
        get => _itemsSource;
        set => SetAndRaise(ItemsSourceProperty, ref _itemsSource, value);
    }

    public Tag SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }
}