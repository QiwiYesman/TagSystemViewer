using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using TagSystemViewer.ViewModels;
using TagSystemViewer.Views;
using Microsoft.Extensions.DependencyInjection;
using SQLite;
using TagSystemViewer.Models;
using TagSystemViewer.Services;
using TagSystemViewer.ViewModels.Observables;

namespace TagSystemViewer;

public partial class App : Application
{
    public readonly string DefaultConfigPath = "database_config.json";
    public readonly string DefaultHotkeyPath = "hotkey_config.json";
    private ResourceDictionary? _hotkeyDictionary;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
       
        var isWindows = OperatingSystem.IsWindows();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new UrlViewer();
            var services = new ServiceCollection();

            services.AddSingleton<FileService>(x => new FileService(desktop.MainWindow));
            services.AddSingleton<ClipboardService>(x => new ClipboardService(desktop.MainWindow)
            {
                ContentDataFormat = isWindows ? DataFormats.Files: "text/uri-list"
            });
            
            Services = services.BuildServiceProvider();
            desktop.MainWindow.Loaded += (sender, args) =>
            {
                try
                {
                    DatabaseConfig = DatabaseConfig.FromFile(DefaultConfigPath);
                    if (DatabaseConfig.CurrentName == "")
                    {
                        DatabaseConfig.CurrentName = DatabaseConfig.Keys.First();
                    }
                }
                catch
                {
                    DatabaseConfig = new();
                    var newWindow = new DatabaseConfigEditor();
                    newWindow.Show(desktop.MainWindow);
                }

                desktop.MainWindow.DataContext = new UrlViewerViewModel();
                var hotkey = new ObservableHotkeyConfig();
                try
                {
                    hotkey.Read(DefaultHotkeyPath);
                    ApplyHotkeys(hotkey.ActiveMap);
                }
                catch (Exception e)
                {
                    hotkey.LoadDefault();
                    hotkey.Save(DefaultHotkeyPath);
                    ApplyHotkeys(hotkey.ActiveMap);
                    
                }
            };
        }
        
        base.OnFrameworkInitializationCompleted();
    }

    public void ApplyHotkeys(List<ObservableHotkeyName> hotkeys)
    {
        if (hotkeys.Count == 0) return;
        if (_hotkeyDictionary is null)
        {
            var dicts = Resources.MergedDictionaries;
            foreach (var resourceProvider in dicts)
            {
                var d = (ResourceDictionary)resourceProvider;
                if (!d.ContainsKey(hotkeys[0].ResourceName)) continue;
                _hotkeyDictionary = d;
                break;
            }
        }

        if (_hotkeyDictionary is null) return;
        foreach (var hotkey in hotkeys)
        {
            _hotkeyDictionary[hotkey.ResourceName] = hotkey.Keys;
        }


    }
    public IServiceProvider? Services { get; private set; }
    public DatabaseConfig DatabaseConfig { get; set; }
    public SQLiteConnection? Connection => Database.CurrentConnection(DatabaseConfig);
    public new static App? Current => Application.Current as App;
}