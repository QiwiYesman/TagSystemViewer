using System;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using TagSystemViewer.ViewModels;
using TagSystemViewer.Views;
using Microsoft.Extensions.DependencyInjection;
using SQLite;
using TagSystemViewer.Models;
using TagSystemViewer.Services;

namespace TagSystemViewer;

public partial class App : Application
{
    public string DefaultConfigPath = "database_config.json";
    
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
                    DatabaseConfig = DatabaseConfig.FromJsonFile(DefaultConfigPath);
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
            };
        }
        
        base.OnFrameworkInitializationCompleted();
    }
    public IServiceProvider? Services { get; private set; }
    public DatabaseConfig DatabaseConfig { get; set; }
    public SQLiteConnection? Connection => Database.CurrentConnection(DatabaseConfig);
    public new static App? Current => Application.Current as App;
}