using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using TagSystemViewer.Models;
using TagSystemViewer.Services;
using TagSystemViewer.ViewModels.Observables;

namespace TagSystemViewer.ViewModels;

public class DatabaseConfigViewModel: ViewModelBase
{
    public DatabaseConfigViewModel()
    {
        Read();
    }
    public ObservableCollection<ObservableDatabaseName> DatabaseNames { get; set; } = new();
    private ObservableDatabaseName? _selectedDatabaseName;
    private string? _newName, _dbPath;

    public void Read()
    {
        var config = App.Current?.DatabaseConfig;
        if (config is null) return;
        DatabaseNames.Clear();
        foreach (var pair in config)
        {
            DatabaseNames.Add(new () {Name = pair.Key, Path = pair.Value});
        }

        if (config.IsEmpty) return;
        SelectedDatabaseName = DatabaseNames.First(x => x.Name == config.CurrentName);
    }
    public string? NewName
    {
        get => _newName;
        set => this.RaiseAndSetIfChanged(ref _newName, value);
    }
    
    public ObservableDatabaseName? SelectedDatabaseName
    {
        get => _selectedDatabaseName;
        set => this.RaiseAndSetIfChanged(ref _selectedDatabaseName, value);
    }

    private bool NameInList(string name) => 
        DatabaseNames.Any(databaseName => databaseName.Name == name);

    public void AddName()
    {
        if (string.IsNullOrEmpty(NewName) || NameInList(NewName)) return;
        var dbName = new ObservableDatabaseName() { Name = NewName };
        DatabaseNames.Add(dbName);
        SelectedDatabaseName = DatabaseNames[^1];
        
    }
    public void UpdateName()
    {
        if (SelectedDatabaseName is null ||
            string.IsNullOrEmpty(NewName) ||
            NameInList(NewName)) return;
        SelectedDatabaseName.Name = NewName;
    }
    public void RemoveName()
    {
        if (SelectedDatabaseName is null) return;
        DatabaseNames.Remove(SelectedDatabaseName);
    }

    public async void BrowseDatabase()
    {
        if (SelectedDatabaseName is null) return;
        var fileService = App.Current?.Services?.GetService<FileService>();
        if(fileService is null) return;
        var file = await fileService.OpenFileDialog();
        if (file is null) return;
        SelectedDatabaseName.Path = Uri.UnescapeDataString(file.Path.AbsolutePath);
        this.RaisePropertyChanged(nameof(SelectedDatabaseName.Name));
    }

    public void Confirm()
    {
        var app = App.Current;
        if (app is null) return;
        var config = app.DatabaseConfig;
        config.Clear();
        foreach (var database in DatabaseNames)
        {
            config[database.Name] = database.Path;
        }

        if (SelectedDatabaseName is null)
        {
            if (!config.IsEmpty)
            {
                config.CurrentName = config.First().Key;
            }
        }
        else
        {
            config.CurrentName = SelectedDatabaseName.Name;
        }
        config.Save(app.DefaultConfigPath);
    }

    public void Refresh()
    {
        var app = App.Current;
        if (app is null) return;
        var config = DatabaseConfig.FromFile(app.DefaultConfigPath);
        DatabaseNames.Clear();
        foreach (var pair in config)
        {
            DatabaseNames.Add(new () {Name = pair.Key, Path = pair.Value});
        }

        app.DatabaseConfig = config;
        if (config.IsEmpty) return;
        SelectedDatabaseName = DatabaseNames.First(x => x.Name == config.CurrentName);
    }

    public async void RemoveDatabase()
    {
        if (SelectedDatabaseName is null) return;
        var fileService = App.Current?.Services?.GetService<FileService>();
        if (fileService is null) return;
        await fileService.RemoveFile(SelectedDatabaseName.Path);
    }

    public async void CreateNewDatabase()
    {
        if (SelectedDatabaseName is null) return;
        var fileService = App.Current?.Services?.GetService<FileService>();
        if (fileService is null) return;
        var file = await fileService.SaveFileDialog();
        if (file is null) return;
        var path = Uri.UnescapeDataString(file.Path.AbsolutePath);
        var conn = Database.DbNewConnection(path);
        Database.CreateTables(conn);
        SelectedDatabaseName.Path = path;
    }
}