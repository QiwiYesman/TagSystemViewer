using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using SQLiteNetExtensions.Extensions;
using TagSystemViewer.Models;
using TagSystemViewer.Services;

namespace TagSystemViewer.ViewModels;

public class UrlViewerViewModel: ViewModelBase
{
    private TagInputSearchViewModel _urlSearch;
    private bool _play = false;

    public TagInputSearchViewModel UrlSearchViewModel
    {
        get => _urlSearch;
        set => this.RaiseAndSetIfChanged(ref _urlSearch, value);
    }
    public bool ToPlayGifs
    {
        get => _play;
        set => this.RaiseAndSetIfChanged(ref _play, value);
    }

    public UrlViewerViewModel()
    {
        FoundUrls = new();
        UrlSearchViewModel = new();
    }
    public ObservableCollection<Url> FoundUrls { get; set; }
    
    public async Task SearchUrls()
    {
        FoundUrls.Clear();
        await Task.Run(()=>
        {
            var list = UrlSearchViewModel.Search();
            foreach (var url in list)
            {
                FoundUrls.Add(url);
            }
        });
        
    }

    public void CopyPath(object arg)
    {
        var path = arg.ToString() ?? "";
        var clipboard = App.Current?.Services?.GetService<ClipboardService>();
        clipboard?.SetTextAsync(path);
    }

    public async Task CopyFile(object arg)
    {
        string path = arg.ToString() ?? "";
        if (!Uri.TryCreate(path, UriKind.Absolute, out var uri)) return;
        var clipboard = App.Current?.Services?.GetService<ClipboardService>();
        if (clipboard is null) return;
        await clipboard.SetFileAsync(uri.AbsoluteUri);
    }
    public void OpenFolder(object arg)
    {
        string path = arg.ToString() ?? "";
        FileProcess.StartDirectory(path);
    }
    

    public void OpenFile(object arg)
    {
        string path = arg.ToString() ?? "";
        FileProcess.StartFile(path);
    }

    public void RefreshTags()
    {
        UrlSearchViewModel = new();
    }
}