using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;
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
    public async void ReadTags()
    {
        var conn = App.Current?.Connection;
        if(conn is null) return;
        var items = await conn.SelectAll<Tag>().ToListAsync();
        FullClear();
        foreach (var tag in items)
        {
            if(tag is null) continue;
            Tags.Add(tag);
        }
    }

    public void ExcludeUpdates()
    {
        if (CurrentUpdateTag is null) return;
        string name = _history[CurrentUpdateTag.Id];
        if (InTagList(name)) return;
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
        if (CurrentRemoveTag is null || InTagList(CurrentRemoveTag.Name)) return;
        Tags.Add(CurrentRemoveTag);
        TagsToRemove.Remove(CurrentRemoveTag);
        
    }

    private bool InTagList(string name) => Tags.Any(tag => tag.Name == name);
    
    public void AddCurrentToAdds()
    {
        if (string.IsNullOrEmpty(NewName) || InTagList(NewName)) return;
        var newTag = new Tag{ Id = 0, Name = NewName };
        TagsToAdd.Add(newTag);
        Tags.Add(newTag);
    }
    public void AddCurrentToUpdates()
    {
        if (string.IsNullOrEmpty(NewName) || CurrentTag is null ||
            CurrentTag.Id==0 || InTagList(NewName)) return;
        var tag = CurrentTag;
        var newTag = new Tag { Id = tag.Id, Name = NewName };
        bool contains = false;
        for (int i = 0; i < TagsToUpdate.Count; i++)
        {
            if (TagsToUpdate[i].Id != newTag.Id) continue;
            TagsToUpdate.RemoveAt(i);
            contains = true;
        }

        if (!contains)
        {
            _history[tag.Id] = tag.Name;
        }

        Tags.Remove(tag);

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

    public void RemoveCascade(SQLiteAsyncConnection conn)
    {
        foreach (var tag in TagsToRemove)
        {
            conn.DeleteAsync(tag.Id, recursive: true);
        }
    }
    public async void Confirm()
    {
        var conn = App.Current?.Connection;
        if(conn is null) return;
        await conn.UpdateAllAsync(TagsToUpdate);
        await conn.InsertAllAsync(TagsToAdd);
        RemoveCascade(conn);
        ReadTags();
    }
}