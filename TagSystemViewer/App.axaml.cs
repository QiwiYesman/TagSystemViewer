using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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
    public const string DefaultConfigPath = "database_config.json";
    public const string DefaultHotkeyPath = "hotkey_config.json";
    private ResourceDictionary? _hotkeyDictionary;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new UrlViewer();
            var services = new ServiceCollection();

            services.AddSingleton<FileService>(_ => new FileService(desktop.MainWindow));
            services.AddSingleton<ClipboardService>(_ => new ClipboardService(desktop.MainWindow));
            
            Services = services.BuildServiceProvider();
            desktop.MainWindow.Loaded += (_, _) =>
            {
                try
                {
                    DatabaseConfig = DatabaseConfig.FromFile(DefaultConfigPath);
                    if (string.IsNullOrEmpty(DatabaseConfig.CurrentName))
                    {
                        DatabaseConfig.CurrentName = DatabaseConfig.Keys.First();
                    }

                    if (!DatabaseConfig.IsAccessibleCurrent())
                    {
                        if (!DatabaseConfig.PickCorrectPath())
                        {
                            DatabaseConfig.CreateDefault();
                        }
                    }
                }
                catch
                {
                    DatabaseConfig = new();
                    var newWindow = new DatabaseConfigEditor();
                    newWindow.Show(desktop.MainWindow);
                }

                desktop.MainWindow.DataContext = new UrlViewerViewModel();
                LoadHotkeys();
            };
        }
        
        base.OnFrameworkInitializationCompleted();
    }

    private void LoadHotkeys()
    {
        var hotkey = new ObservableHotkeyConfig();
        try
        {
            hotkey.Read(DefaultHotkeyPath);
            ApplyHotkeys(hotkey.ActiveMap);
        }
        catch
        {
            hotkey.LoadDefault();
            hotkey.Save(DefaultHotkeyPath);
            ApplyHotkeys(hotkey.ActiveMap);
                    
        }
    }
    public void ApplyHotkeys(List<ObservableHotkeyName> hotkeys)
    {
        if (hotkeys.Count == 0) return;
        if (_hotkeyDictionary is null)
        {
            var mergedDictionaries = Resources.MergedDictionaries;
            foreach (var resourceProvider in mergedDictionaries)
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
    public DatabaseConfig? DatabaseConfig { get; set; }
    public SQLiteConnection? Connection =>DatabaseConfig is null ? null: Database.CurrentConnection(DatabaseConfig);
    public new static App? Current => Application.Current as App;
}