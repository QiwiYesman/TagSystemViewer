using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Input;
using ReactiveUI;
using TagSystemViewer.Utility;
using TagSystemViewer.ViewModels.Observables;

namespace TagSystemViewer.ViewModels;

public class HotkeyEditorViewModel: ViewModelBase
{
    public ObservableHotkeyConfig Config { get; set; }
    private readonly bool[] _isModifiers = new bool[3];
    private ObservableHotkeyName? _currentHotkey;
    private List<ObservableHotkeyName>? _currentHotkeys;
    private List<string>? _configNames; 
    private bool _isEditing = false;
    private string _keysString = "", _newName = "";
    private string? _currentName;

    public string? CurrentConfigName
    {
        get => _currentName;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentName, value);
            if (_currentName is null)
            {
                CurrentHotkeys = new();
                return;
            }
            CurrentHotkeys = Config.Get(_currentName);
        }
    }
    
    public bool IsAlt
    {
        get => _isModifiers[2];
        set => this.RaiseAndSetIfChanged(ref _isModifiers[2], value);
    }
    public bool IsShift
    {
        get => _isModifiers[1];
        set => this.RaiseAndSetIfChanged(ref _isModifiers[1], value);
    }

    public bool IsCtrl
    {
        get => _isModifiers[0];
        set => this.RaiseAndSetIfChanged(ref _isModifiers[0], value);
    }

    public ObservableHotkeyName? CurrentHotkey
    {
        get => _currentHotkey;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentHotkey, value);
            if (_currentHotkey == null) return;
            SetModifiersBools(_currentHotkey.Keys);
            KeysString = _currentHotkey.Keys.Key.ToString();
        }
    }

    public List<ObservableHotkeyName>? CurrentHotkeys
    {
        get => _currentHotkeys;
        set => this.RaiseAndSetIfChanged(ref _currentHotkeys, value);
    }
    public List<string>? ConfigNames
    {
        get => _configNames;
        set => this.RaiseAndSetIfChanged(ref _configNames, value);
    }
    public bool IsEditing
    {
        get => _isEditing;
        set => this.RaiseAndSetIfChanged(ref _isEditing, value);
    }
    public string KeysString
    {
        get => _keysString;
        set => this.RaiseAndSetIfChanged(ref _keysString, value);
    }
    public string NewName
    {
        get => _newName;
        set => this.RaiseAndSetIfChanged(ref _newName, value);
    }

    public void UpdateCurrentHotkey()
    {
        if (CurrentHotkey is null) return;
        if (!Enum.TryParse(KeysString, out Key key)) return;
        var keyModifier = KeyModifiers.None;
        if (IsAlt) keyModifier |= KeyModifiers.Alt;
        if (IsCtrl) keyModifier |= KeyModifiers.Control;
        if (IsShift) keyModifier |= KeyModifiers.Shift;
        
        CurrentHotkey.Keys = new KeyGesture(key, keyModifier);
    }

    public void Confirm()
    {
        var app = App.Current;
        if (app is null || CurrentConfigName is null) return;
        Config.ActiveName = CurrentConfigName;
        Config.Save(App.DefaultHotkeyPath);
        app.ApplyHotkeys(Config.ActiveMap);
        Read();
    }

    public void AddNewMap()
    {
        if (CurrentConfigName is null || CurrentHotkeys is null) return;
        Config.Add(NewName, CurrentHotkeys);
        ConfigNames = Config.KeyNames;
        CurrentConfigName = NewName;
    }

    public void RemoveCurrentMap()
    {
        if (CurrentConfigName is null) return;
        Config.Remove(CurrentConfigName);
        ConfigNames = Config.KeyNames;
        CurrentConfigName = Config.ActiveName;
        
    }

    public void UpdateCurrentMap()
    {
        if (CurrentConfigName is null) return;
        Config.Update(CurrentConfigName, NewName);
        ConfigNames = Config.KeyNames;
    }
    
    private void SetModifiersBools(KeyGesture keys)
    {
        var modifiers = keys.KeyModifiers;
        IsCtrl = modifiers.HasFlag(KeyModifiers.Control);
        IsShift = modifiers.HasFlag(KeyModifiers.Shift);
        IsAlt = modifiers.HasFlag(KeyModifiers.Alt);
    }

    public void Read()
    {
        CurrentHotkeys =null;
        ConfigNames = null;
        Config = new();
        var app = App.Current;
        if (app is null) return;
        try
        {
            Config.Read(App.DefaultHotkeyPath);
        }
        catch (Exception e)
        {
            Config.LoadDefault();
        }
        finally
        {
            if(Config.Config.Count ==0) Config.LoadDefault();
            ConfigNames = Config.KeyNames;
            CurrentConfigName = Config.ActiveName;
        }
    }
    public HotkeyEditorViewModel()
    {
        ReadAsync();
    }

    public async Task UpdateCurrentMapAsync() => await AsyncLauncher.LaunchDispatcher(UpdateCurrentMap);
    public async Task AddNewMapAsync() => await AsyncLauncher.LaunchDispatcher(AddNewMap);
    public async Task RemoveCurrentMapAsync() => await AsyncLauncher.LaunchDispatcher(RemoveCurrentMap);
    public async Task UpdateCurrentHotkeyAsync() => await AsyncLauncher.LaunchDispatcher(UpdateCurrentHotkey);
    public async Task ConfirmAsync() => await AsyncLauncher.LaunchDispatcher(Confirm);
    public async void ReadAsync() => await AsyncLauncher.LaunchTask(Read);
}