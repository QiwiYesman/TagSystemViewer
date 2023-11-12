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

    public TagInputSearchViewModel UrlSearchViewModel
    {
        get => _urlSearch;
        set => this.RaiseAndSetIfChanged(ref _urlSearch, value);
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
        string path = arg.ToString() ?? "";
        var clipboard = App.Current?.Services?.GetService<ClipboardService>();
        clipboard?.SetTextAsync(path);
    }

    public async Task CopyFile(object arg)
    {
        string path = arg.ToString() ?? "";
        var clipboard = App.Current?.Services?.GetService<ClipboardService>();
        if (clipboard is null) return;
        var dataObject = new DataObject();
        dataObject.Set(DataFormats.Files, path);
        await clipboard.SetDataObjectAsync(dataObject);
    }
    public void OpenFolder(object arg)
    {
        string path = arg.ToString() ?? "";
        var folder = Path.GetDirectoryName(path);
        if (folder is null) return;
        StartProcess(folder);
    }

    public static void StartProcess(string path)
    {
        if (path == "") return;
        Process.Start( new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        });
    }
    public void OpenFile(object arg)
    {
        string path = arg.ToString() ?? "";
        StartProcess(path);
    }
}