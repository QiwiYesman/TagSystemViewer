using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using TagSystemViewer.Models;
using TagSystemViewer.Services;
using TagSystemViewer.Utility;

namespace TagSystemViewer.ViewModels;

public class UrlViewerViewModel: ViewModelBase
{
    private TagInputSearchViewModel _urlSearch = new();
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
    public ObservableCollection<Url> FoundUrls { get; set; } = new();
    
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
    public static void OpenFolder(object arg) =>
        FileProcess.StartDirectory(arg.ToString() ?? "");
    

    public static void OpenFile(object arg)=>
        FileProcess.StartFile(arg.ToString() ?? "");

    public void RefreshTags() => UrlSearchViewModel.ReadTags();
}