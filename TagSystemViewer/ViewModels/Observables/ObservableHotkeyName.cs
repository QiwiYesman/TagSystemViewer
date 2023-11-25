using System;
using System.Collections.Specialized;
using Avalonia.Input;
using ReactiveUI;

namespace TagSystemViewer.ViewModels.Observables;

public class ObservableHotkeyName: ViewModelBase, ICloneable
{
    private KeyGesture _keys;
    public string Name { get; set; }

    public KeyGesture Keys
    {
        get => _keys;
        set => this.RaiseAndSetIfChanged(ref _keys, value);

    }
    public string ResourceName { get; set; }

    public ObservableHotkeyName()
    {
    }

    public ObservableHotkeyName(string? name, string? key, string? resources)
    {
        if(name!=null) Name = name;
        if(key!=null) Keys = KeyGesture.Parse(key);
        if(resources!=null) ResourceName = resources;
    }

    public string[] ToArray() => new string[] {Name, Keys.ToString(), ResourceName};
    public static ObservableHotkeyName FromArray(string[] arr) => new(arr[0], arr[1], arr[2]);

    public object Clone() =>
        new ObservableHotkeyName() { Name = Name, Keys = Keys, ResourceName = ResourceName };
}