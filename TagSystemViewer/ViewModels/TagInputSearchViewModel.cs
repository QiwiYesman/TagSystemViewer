using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData.Binding;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using TagSystemViewer.Enums;
using TagSystemViewer.Models;
using TagSystemViewer.Services;

namespace TagSystemViewer.ViewModels;

public class TagInputSearchViewModel: ViewModelBase
{
    public TagInputSearchViewModel()
    {
        ReadTags();
    }
    private Tag? _currentTag, _currentListTag;
    private TagSearchStates _currentStates = TagSearchStates.Or;
    public ObservableCollection<Tag> Tags { get; set; } = new();
    public ObservableCollection<Tag> AndTags { get; set; } = new();
    public ObservableCollection<Tag> NotTags { get; set; } = new();
    public ObservableCollection<Tag> OrTags { get; set; } = new();
    
    public Tag? CurrentTag
    {
        get => _currentTag;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentTag, value);
            _currentListTag = value;
            this.RaisePropertyChanged(nameof(CurrentListTag));
        } 
    }
    public Tag? CurrentListTag
    {
        get => _currentListTag;
        set
        {
            if (value is null) return;
            this.RaiseAndSetIfChanged(ref _currentListTag, value);
            if(Equals(_currentTag, value)) return;
            _currentTag = value;
            this.RaisePropertyChanged(nameof(CurrentTag));
            
        } 
    }
    public TagSearchStates CurrentStates
    {
        get => _currentStates;
        set =>this.RaiseAndSetIfChanged(ref _currentStates, value);
    }
    
    public void ReadTags()
    {
        var conn = App.Current?.Connection;
        if(conn is null) return;
        var items = conn.SelectAll<Tag>();
        Tags.Clear();
        foreach (var i in items)
        {
            Tags.Add(i);
        }
    }

    public static void RemoveIfExist(Tag tag, ObservableCollection<Tag> list)
    {
        var i = list.IndexOf(tag);
        if (i != -1)
        {
            list.RemoveAt(i);
        }
    }
    
    public void AddTag()
    {
        if(CurrentTag is null) return;
        var tag = CurrentTag;
        switch (CurrentStates)
        {
            case TagSearchStates.And:
                if (AndTags.Contains(CurrentTag)) return;
                AndTags.Add(tag);
                RemoveIfExist(tag, NotTags);
                RemoveIfExist(tag, OrTags);
                break;
            case TagSearchStates.Or:
                if (OrTags.Contains(CurrentTag)) return;
                OrTags.Add(tag);
                RemoveIfExist(tag, NotTags);
                RemoveIfExist(tag, AndTags);
                break;
            case TagSearchStates.Not:
                if (NotTags.Contains(CurrentTag)) return;
                NotTags.Add(tag);
                RemoveIfExist(tag, AndTags);
                RemoveIfExist(tag, OrTags);
                break;
        }
    }

    public void RemoveTag()
    {
        if (CurrentTag is null) return;
        var tag = CurrentTag;
        foreach (var list in new []{AndTags, OrTags, NotTags})
        {
            var i = list.IndexOf(tag);
            if (i == -1) continue;
            list.RemoveAt(i);
            return;
        }
    }
    public List<Url> Search()
    {
        var conn = App.Current?.Connection;
        if(conn is null) return new();
        return conn.SelectUrls(new()
        {
            AndTags = AndTags,
            OrTags = OrTags,
            NotTags = NotTags
        });
    }

    public void ClearAll()
    {
        AndTags.Clear();
        OrTags.Clear();
        NotTags.Clear();
    }
}