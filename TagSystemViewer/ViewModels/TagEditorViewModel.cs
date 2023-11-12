using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using SQLite;
using SQLiteNetExtensions.Extensions;
using TagSystemViewer.Models;

namespace TagSystemViewer.ViewModels;

public class TagEditorViewModel : ViewModelBase
{
    private string _newName;
    private Tag?[] _currentNames;
    private Dictionary<int, string> _history = new();
    public TagEditorViewModel()
    {
        _currentNames = new Tag[4];
        ReadTags();
    }

    public ObservableCollection<Tag> Tags { get; set; } = new();
    public ObservableCollection<Tag> TagsToRemove { get; set; } = new();
    public ObservableCollection<Tag> TagsToUpdate { get; set; } = new();
    public ObservableCollection<Tag> TagsToAdd { get; set; } = new();
    
    public Tag? CurrentRemoveTag
    {
        get => _currentNames[3];
        set => this.RaiseAndSetIfChanged(ref _currentNames[3], value);
    }
    public Tag? CurrentUpdateTag
    {
        get => _currentNames[2];
        set => this.RaiseAndSetIfChanged(ref _currentNames[2], value);
    }
    public Tag? CurrentAddTag
    {
        get => _currentNames[1];
        set => this.RaiseAndSetIfChanged(ref _currentNames[1], value);
    }
    public Tag? CurrentTag
    {
        get => _currentNames[0];
        set => this.RaiseAndSetIfChanged(ref _currentNames[0], value);
    }
    
    public string NewName
    {
        get => _newName;
        set => this.RaiseAndSetIfChanged(ref _newName, value);
    }

    public void FullClear()
    {
        Tags.Clear();
        TagsToAdd.Clear();
        TagsToUpdate.Clear();
        TagsToRemove.Clear();
    }
    public void ReadTags()
    {
        var conn = Database.DbExistingConnection();
        var items = conn.SelectAll<Tag>();
        FullClear();
        foreach (var i in items)
        {
            Tags.Add(i);
        }
    }

    public void ExcludeUpdates()
    {
        if (CurrentUpdateTag is null) return;
        string name = _history[CurrentUpdateTag.Id];
        if (InList(name)) return;
        Tags.Add(new Tag{Id=CurrentUpdateTag.Id, Name = name});
        _history.Remove(CurrentUpdateTag.Id);
        Tags.Remove(CurrentUpdateTag);
        TagsToUpdate.Remove(CurrentUpdateTag);
    }

    public void ExcludeAdds()
    {
        if (CurrentAddTag is null) return;
        Tags.Remove(CurrentAddTag);
        TagsToAdd.Remove(CurrentAddTag);
    }

    public void ExcludeRemoves()
    {
        if (CurrentRemoveTag is null || InList(CurrentRemoveTag.Name)) return;
        Tags.Add(CurrentRemoveTag);
        TagsToRemove.Remove(CurrentRemoveTag);
        
    }

    bool InList(string name) 
        => Tags.Any(tag => tag.Name == name);
    
    public void AddCurrentToAdds()
    {
        if (string.IsNullOrEmpty(NewName) || InList(NewName)) return;
        var newTag = new Tag{ Id = 0, Name = NewName };
        TagsToAdd.Add(newTag);
        Tags.Add(newTag);
    }
    public void AddCurrentToUpdates()
    {
        if (string.IsNullOrEmpty(NewName) || CurrentTag is null ||
            CurrentTag.Id==0 || InList(NewName)) return;
        var newTag = new Tag { Id = CurrentTag.Id, Name = NewName };
        _history[CurrentTag.Id] = CurrentTag.Name;
        Tags.Remove(CurrentTag);
        for (int i = 0; i < TagsToUpdate.Count; i++)
        {
            if (TagsToUpdate[i].Id != newTag.Id) continue;
            TagsToUpdate.RemoveAt(i);
            return;
        }

        TagsToUpdate.Add(newTag);
        Tags.Add(newTag);
    }
    public void AddCurrentToRemoves()
    {
        if (CurrentTag is null) return;
        if (CurrentTag.Id != 0)
        {
            TagsToRemove.Add(new Tag()
            {
                Id = CurrentTag.Id,
                Name = CurrentTag.Name
            });
        }
        else
        {
            TagsToAdd.Remove(CurrentTag);
        }
        Tags.Remove(CurrentTag);
    }

    public void RemoveCascade(SQLiteConnection conn)
    {
        foreach (var tag in TagsToRemove)
        {
            conn.Delete(tag.Id, recursive: true);
        }
    }
    public void Confirm()
    {
        var conn = Database.DbExistingConnection();
        conn.UpdateAll(TagsToUpdate);
        conn.InsertAll(TagsToAdd);
        RemoveCascade(conn);
        ReadTags();
    }
}