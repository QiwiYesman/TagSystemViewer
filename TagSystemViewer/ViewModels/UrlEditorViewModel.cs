using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using SQLite;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensionsAsync.Extensions;
using TagSystemViewer.Enums;
using TagSystemViewer.Models;
using TagSystemViewer.Services;
using TagSystemViewer.Utility;
using TagSystemViewer.ViewModels.Observables;

namespace TagSystemViewer.ViewModels;

public class UrlEditorViewModel: ViewModelBase
{
    private ObservableUrl? _currentUrl;
    private Tag? _currentTag;
    private ObservableCollection<ObservableUrl> _urls;
    private ObservableCollection<Tag> _tags;

    private string? _newName;
    
    public string? NewName
    {
        get => _newName;
        set => this.RaiseAndSetIfChanged(ref _newName, value);
    }
    public ObservableCollection<ObservableUrl> Urls
    {
        get => _urls;
        set => this.RaiseAndSetIfChanged(ref _urls, value);
    }
    public ObservableCollection<Tag> Tags
    {
        get => _tags;
        set => this.RaiseAndSetIfChanged(ref _tags, value);
    }


    public ObservableUrl? CurrentUrl
    {
        get => _currentUrl;
        set => this.RaiseAndSetIfChanged(ref _currentUrl, value);
    }
    public Tag? CurrentTag
    {
        get => _currentTag;
        set => this.RaiseAndSetIfChanged(ref _currentTag, value);
    }

    public UrlEditorViewModel()
    {
        Urls = new();
        ReadTags();
        //AsyncLauncher.LaunchDispatcherVoid(ReadTags);
        //ReadTagsAsync();
    }
    
    public async void ReadTags()
    {
        var conn = App.Current?.Connection;
        if(conn is null) return;
        var urls = await conn.GetAllWithChildrenAsync<Url>();
        Urls.Clear();
        foreach (var url in urls)
        {
            var observableUrl = new ObservableUrl(url);
            var isRelative = LinkResolver.IsRelative(url.Link);
            if (isRelative)
            {
                observableUrl.SaveAsRelative = true;
                observableUrl.CurrentLink = LinkResolver.ResolveAbsoluteUrl(
                    App.Current?.DatabaseConfig?.CurrentPath??"", 
                    observableUrl.CurrentLink);
            }
            Urls.Add(observableUrl);
        }
        var tags = await conn.SelectAll<Tag>().ToArrayAsync();
        Tags = new ObservableCollection<Tag>(tags);
    }

    public void AddTag()
    {
        if (CurrentTag is null || CurrentUrl is null) return;
        if (CurrentUrl.Tags.Any(tag => tag.Name == CurrentTag.Name))
        {
            return;
        }
        CurrentUrl.Tags.Add(CurrentTag);
        ForceMarkUpdate();
    }

    public void RemoveTag()
    {
        if(CurrentTag is null || CurrentUrl is null) return;
        CurrentUrl.Tags.Remove(CurrentTag);
        ForceMarkUpdate();
    }
    
    private bool InList(string name) => 
        Urls.Any(url => url.CurrentLink == name && url.RecordStates != RecordStates.Delete);
    
    public void MarkDeleteUrl()
    {
        if (CurrentUrl is null) return;
        if (CurrentUrl.RecordStates == RecordStates.Insert)
        {
            Urls.Remove(CurrentUrl);
            return;
        }
        CurrentUrl.RecordStates = RecordStates.Delete;
        SetPrevName();
    }
    public void ForceMarkUpdate()
    {
        if (CurrentUrl is null || CurrentUrl.RecordStates == RecordStates.Insert) return;
        CurrentUrl.RecordStates = RecordStates.Update;
    }
    public void MarkUpdateUrl()
    {
        if (CurrentUrl is null || string.IsNullOrEmpty(NewName) ||
            CurrentUrl.RecordStates == RecordStates.Insert ||
            InList(NewName)) return;
        CurrentUrl.RecordStates = RecordStates.Update;
        CurrentUrl.OldName = CurrentUrl.CurrentLink;
        CurrentUrl.CurrentLink = NewName;
    }

    public void MarkInsertUrl()
    {
        if (NewName is null || InList(NewName)) return;
        var url = new ObservableUrl(NewName);
        Urls.Add(url);
        CurrentUrl = url;
    }

    private void SetPrevName()
    {
        if (CurrentUrl?.OldName is null) return;
        CurrentUrl.CurrentLink = CurrentUrl.OldName;
        CurrentUrl.OldName = null;
    }
    public void CancelMark()
    {
        if (CurrentUrl is null) return;
        switch (CurrentUrl.RecordStates)
        {
            case RecordStates.Insert:
                Urls.Remove(CurrentUrl);
                return;
            case RecordStates.Update when CurrentUrl.OldName!= null && InList(CurrentUrl.OldName):
                return;
            default:
                CurrentUrl.RecordStates = RecordStates.NotModified;
                SetPrevName();
                break;
        }
    }

    public async Task BrowseSaveFile()
    {
        var fileService = App.Current?.Services?.GetService<FileService>();
        if(fileService is null) return;
        var file = await fileService.SaveFileDialog();
        if (file is null) return;
        NewName = Uri.UnescapeDataString(file.Path.AbsolutePath);
    }

    private static bool Move(ObservableUrl url)
    {
        return !string.IsNullOrEmpty(url.OldName) &&
               FileProcess.MoveFile(url.OldName, url.CurrentLink);
    }
    public void MarkMoveUrl()
    {  
        if (CurrentUrl is null || string.IsNullOrEmpty(NewName) ||
           CurrentUrl.RecordStates == RecordStates.Insert ||
           CurrentUrl.RecordStates == RecordStates.Update ||
           InList(NewName)) return;
        CurrentUrl.RecordStates = RecordStates.UpdateAndMove;
        CurrentUrl.OldName = CurrentUrl.CurrentLink;
        CurrentUrl.CurrentLink = NewName;
    }
    public void StartFile()
    {
        if (CurrentUrl is null) return;
        FileProcess.StartFile(CurrentUrl.CurrentLink);
    }

    public async Task BrowseFile()
    {
        var fileService = App.Current?.Services?.GetService<FileService>();
        if(fileService is null) return;
        var file = await fileService.OpenFileDialog();
        if (file is null) return;
        NewName = Uri.UnescapeDataString(file.Path.AbsolutePath);
    }

    private async Task UpdateUrl(SQLiteAsyncConnection conn, ObservableUrl currentUrl)
    {
        var commonUrl = new Url
        {
            Id = currentUrl.Id,
            Link = currentUrl.CurrentLink,
            Tags = currentUrl.Tags.ToList()
        };
        if (currentUrl.SaveAsRelative)
        {
            commonUrl.Link = LinkResolver.GetRelativeUrl(
                App.Current?.DatabaseConfig?.CurrentPath ?? "",
                commonUrl.Link);
        }
        await conn.UpdateWithChildrenAsync(commonUrl);
    }
    public async Task Confirm()
    {
        var conn = App.Current?.Connection;
        if(conn is null) return;
        foreach (var url in Urls)
        {
            Url? commonUrl;
            switch (url.RecordStates)
            {
                case RecordStates.Delete:
                    commonUrl = new Url
                    {
                        Id = url.Id,
                    };
                    await conn.DeleteAsync(commonUrl, true);
                    break;
                case RecordStates.UpdateAndMove:
                    if (Move(url))
                    {
                        await UpdateUrl(conn, url);
                    }

                    break;
                case RecordStates.Update:
                    await UpdateUrl(conn, url);
                    break;
                case RecordStates.Insert:
                    commonUrl = new Url
                    {
                        Link = url.CurrentLink,
                        Tags = url.Tags.ToList()
                    };
                    await conn.InsertWithChildrenAsync(commonUrl);
                    break;
            }
        }
        ReadTags();
    }

    public void SwitchRelativeUrl()
    {
        if (CurrentUrl is null) return;
        CurrentUrl.SaveAsRelative = !CurrentUrl.SaveAsRelative;
        ForceMarkUpdate();
    }

    public async void ReadTagsAsync() => ReadTags(); 
        //AsyncLauncher.LaunchDispatcherVoid(ReadTags);
        public async Task ConfirmAsync() => await Confirm();
    // await AsyncLauncher.LaunchDispatcher(Confirm);
    public async Task StartFileAsync() => await AsyncLauncher.LaunchDispatcher(StartFile);
    public async Task MarkMoveUrlAsync() => await AsyncLauncher.LaunchDispatcher(MarkMoveUrl);
    public async Task CancelMarkAsync() => await AsyncLauncher.LaunchDispatcher(CancelMark);
    public async Task MarkInsertUrlAsync() => await AsyncLauncher.LaunchDispatcher(MarkInsertUrl);
    public async Task MarkUpdateUrlAsync() => await AsyncLauncher.LaunchDispatcher(MarkUpdateUrl);
    
    public async Task ForceMarkUpdateAsync() => await AsyncLauncher.LaunchDispatcher(ForceMarkUpdate);
    public async Task RemoveTagAsync() => await AsyncLauncher.LaunchDispatcher(RemoveTag);
    public async Task AddTagAsync() => await AsyncLauncher.LaunchDispatcher(AddTag);
    public async Task MarkDeleteUrlAsync() => await AsyncLauncher.LaunchDispatcher(MarkDeleteUrl);
    public async Task SwitchRelativeUrlAsync() => await AsyncLauncher.LaunchDispatcher(SwitchRelativeUrl);
}