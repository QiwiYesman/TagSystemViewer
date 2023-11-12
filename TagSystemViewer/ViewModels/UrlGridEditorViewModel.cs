using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using SQLiteNetExtensions.Extensions;
using TagSystemViewer.Enums;
using TagSystemViewer.Models;
using TagSystemViewer.Services;

namespace TagSystemViewer.ViewModels;

public class UrlGridEditorViewModel: ViewModelBase
{
    private ObservableUrl? _currentUrl;
    private Tag? _currentTag;
    private ObservableCollection<ObservableUrl> _urls;
    private ObservableCollection<Tag> _tags;

    private string? _newName;
    private bool _correctUrl;
    public bool IsCorrectUrl
    {
        get => _correctUrl;
        set => this.RaiseAndSetIfChanged(ref _correctUrl, value);
    }
    
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

    public UrlGridEditorViewModel()
    {
        Urls = new();
        ReadTags();
    }
    
    public void ReadTags()
    {
        var conn = Database.DbExistingConnection();
        var urls = conn.GetAllWithChildren<Url>();
        Urls.Clear();
        foreach (var url in urls)
        {
            Urls.Add(new ObservableUrl(url));
        }
        var tags = conn.SelectAll<Tag>();
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

    public async void CheckConnection()
    {
        if (CurrentUrl is null) return;
        try
        {
            
            using var client = new HttpClient();
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, CurrentUrl.CurrentLink));
            IsCorrectUrl =  response.StatusCode == HttpStatusCode.OK;
        }            
        catch
        {
            IsCorrectUrl = File.Exists(CurrentUrl.CurrentLink);
        }
        
    }

    public async Task BrowseFile()
    {
        var fileService = App.Current?.Services?.GetService<FileService>();
        if(fileService is null) return;
        var file = await fileService.OpenFileDialog();
        if (file is null) return;
        NewName = Uri.UnescapeDataString(file.Path.AbsolutePath);
    }
    public void Confirm()
    {
        var connection = Database.DbExistingConnection();
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
                    connection.Delete(commonUrl, true);
                    break;
                case RecordStates.Update:
                    commonUrl = new Url
                    {
                        Id = url.Id,
                        Link = url.CurrentLink,
                        Tags = url.Tags.ToList()
                    };
                    connection.UpdateWithChildren(commonUrl);
                    break;
                case RecordStates.Insert:
                    commonUrl = new Url
                    {
                        Link = url.CurrentLink,
                        Tags = url.Tags.ToList()
                    };
                    connection.InsertWithChildren(commonUrl);
                    break;
            }
        }
        ReadTags();
    }
    
}