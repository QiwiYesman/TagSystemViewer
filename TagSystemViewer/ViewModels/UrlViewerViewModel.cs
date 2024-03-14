using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
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
        Console.WriteLine("searching");
        var list = await UrlSearchViewModel.Search();
        Console.WriteLine("found, starting fill");
//        await Dispatcher.UIThread.InvokeAsync(() => 
        FillFoundUrls(list);
        //);
        /*FoundUrls.Clear();
        foreach (var url in list)
        {
            FoundUrls.Add(url);
        }*/
    }

    private void FillFoundUrls(IEnumerable<Url> urls)
    {
            FoundUrls.Clear();
            foreach (var url in urls)
            {
                var isRelative = LinkResolver.IsRelative(url.Link);
                if (isRelative)
                {
                    url.Link = LinkResolver.ResolveAbsoluteUrl(
                        App.Current?.DatabaseConfig?.CurrentPath??"", 
                        url.Link);
                }
                FoundUrls.Add(url);
            }
            Console.WriteLine("added urls");
    }

    public async void SearchUrlsAsync() => await SearchUrls(); 
        //await AsyncLauncher.LaunchTask(SearchUrls);
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

    public async void RefreshTags() => await UrlSearchViewModel.ReadTags();
}