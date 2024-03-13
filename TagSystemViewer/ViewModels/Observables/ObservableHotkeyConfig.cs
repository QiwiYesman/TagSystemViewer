using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using TagSystemViewer.Utility;
namespace TagSystemViewer.ViewModels.Observables;

public class ObservableHotkeyConfig: ViewModelBase
{
    public Dictionary<string, List<ObservableHotkeyName>> Config { get; set; } = new();
    private string? _activeName;
    
    public string? ActiveName
    {
        get => _activeName;
        set => this.RaiseAndSetIfChanged(ref _activeName, value);
    }

    public List<ObservableHotkeyName> ActiveMap
    {
        get
        {
            _activeName ??= Config.Keys.First();
            return Config[_activeName];
        }
    }
    public List<string> KeyNames => Config.Keys.ToList();

    public void Save(string path)
    {
        if (ActiveName is null) return;
        Dictionary<string, List<string[]>> dict = new();
        foreach (var key in Config.Keys)
        {
            dict[key] = new();
            foreach (var hotkey in Config[key])
            {
                dict[key].Add(hotkey.ToArray());
            }
        }
        Dictionary<string, Dictionary<string, List<string[]>>> named = new()
        {
            [ActiveName] = dict
        };
        Serializer.SerializeToFileJson(named, path);
    }

    public void Read(string path)
    {
        var obj = Serializer.DeserializeFromFileJson<Dictionary<string, Dictionary<string, List<string[]>>>>(path);
        if(obj is null) return;

        var dict = obj.Values.First();
        Config = new();
        foreach (var pair in dict)
        {
            Config[pair.Key] = pair.Value.Select(ObservableHotkeyName.FromArray).ToList();
        }

        ActiveName = obj.Keys.First();
    }
  
    
    public void LoadDefault()
    {
        Config["default"] = new List<ObservableHotkeyName>()
        {
            new("Вийти без змін", "escape","ExitHotkey"),
            new("Прийняти зміни", "ctrl+s","ConfirmHotkey"),
            new("Оновити поточні списки\n Зчитання з бази даних/з файлу", "ctrl+r","RefreshHotkey"),
            new("Очистити тегові списки при пошуку", "ctrl+shift+r","ClearHotkey"),
            new("Поставити позначку оновлення у редакторі посилань\n Оновлення комбінації в редакторі гарячих клавіш", "ctrl+shift+up","ForceUpdateHotkey"),
            new("Скасувати позначку", "ctrl+z","CancelMarkHotkey"),
            new("Оновити поточне значення", "ctrl+up","UpdateHotkey"),
            new("Запустити посилання програмою за замовчуванням", "ctrl+shift+enter","LaunchHotkey"),
            new("Відкрити папку для пошуку", "ctrl+shift+a","OpenFolderHotkey"),
            new("Запустити вікно для редагування тегів", "ctrl+d1","OpenTagsHotkey"),
            new("Запустити вікно для редагування посилань", "ctrl+d2","OpenUrlsHotkey"),
            new("Запустити вікно для редагування баз даних", "ctrl+d3","OpenDatabaseHotkey"),
            new("Запустити вікно для редагування гарячих клавіш", "ctrl+d4","OpenHotkeysHotkey"),
            new("Обрати список 'i'", "ctrl+q","ToAndTagHotkey"),
            new("Обрати список 'або'", "ctrl+w","ToOrTagHotkey"),
            new("Обрати список 'не'", "ctrl+e","ToNotTagHotkey"),
            new("Відтворити/поставити на паузу контент", "ctrl+d","ToPlayPauseHotkey"),
            new("Пошук посилань", "ctrl+enter","SearchHotkey"),
            new("Створити базу даних", "ctrl+insert","CreateHotkey"),
            new("Додати до списку", "insert","AddHotkey"),
            new("Прив'язати тег до посилання", "ctrl++","AddSecondHotkey"),
            new("Вилучити зі списку/позначити на видалення", "delete","ExcludeHotkey"),
            new("Відв'язати тег від посилання", "ctrl+-","ExcludeSecondHotkey"),
            new("Видалити базу даних", "ctrl+delete","RemoveHotkey"),
            new("Позначити файл для переміщення", "ctrl+m","MoveFileHotkey"),
            new("Вказати новий файл для переміщення", "ctrl+shift+m","SaveBrowserHotkey"),
            new("Позначити файл для зміни шляху на відносний або навпаки", "ctrl+h","MarkRelativeHotkey"),
        };
        ActiveName = "default";
    }

    public void Remove(string name)
    {
        if (Config.Count == 1) return;
        Config.Remove(name);
        ActiveName = Config.Keys.First();
    }

    public void Add(string name, List<ObservableHotkeyName> hotkeys)
    {
        if (Config.ContainsKey(name)) return;
        Config[name] = hotkeys.Select(x=>(ObservableHotkeyName)x.Clone()).ToList();
    }
    

    public void Update(string oldName, string newName)
    {
        var list = Config[oldName];
        Config[newName] = list;
        Config.Remove(oldName);
        if (oldName == ActiveName) ActiveName = newName;
    }
    
  
    public List<ObservableHotkeyName> Get(string key) => Config[key];
}