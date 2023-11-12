using System.Collections;
using Avalonia;
using Avalonia.Controls.Primitives;
using TagSystemViewer.Models;

namespace TagSystemViewer.Controls;

public class WrapListBox : TemplatedControl
{
    private IEnumerable _ItemsSource;
    public static readonly DirectProperty<WrapListBox, IEnumerable> ItemsSourceProperty = AvaloniaProperty.RegisterDirect<WrapListBox, IEnumerable>("ItemsSource", o => o.ItemsSource, (o, v) => o.ItemsSource = v);
    public static readonly StyledProperty<Tag> SelectedItemProperty = AvaloniaProperty.Register<WrapListBox, Tag>("SelectedItem");

    public IEnumerable ItemsSource
    {
        get => _ItemsSource;
        set => SetAndRaise(ItemsSourceProperty, ref _ItemsSource, value);
    }

    public Tag SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }
}