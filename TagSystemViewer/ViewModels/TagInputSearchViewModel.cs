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
    private Tag? _currentTag;
    private TagSearchStates _currentStates = TagSearchStates.Or;
    private string _currentStateString = "Or";
    public ObservableCollection<Tag> Tags { get; set; } = new();
    public ObservableCollection<Tag> AndTags { get; set; } = new();
    public ObservableCollection<Tag> NotTags { get; set; } = new();
    public ObservableCollection<Tag> OrTags { get; set; } = new();
    
    public Tag? CurrentTag
    {
        get => _currentTag;
        set => this.RaiseAndSetIfChanged(ref _currentTag, value);
    }

    public TagSearchStates CurrentStates
    {
        get => _currentStates;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentStates, value);
            CurrentStateString = value.ToString();
        }
    }

    public string CurrentStateString
    {
        get => _currentStateString;
        set => this.RaiseAndSetIfChanged(ref _currentStateString, value);
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
        Tag? tag;
        switch (CurrentStates)
        {
            case TagSearchStates.And:
                if (AndTags.Contains(CurrentTag)) return;
                tag = CurrentTag;
                AndTags.Add(tag);
                RemoveIfExist(tag, NotTags);
                RemoveIfExist(tag, OrTags);
                break;
            case TagSearchStates.Or:
                if (OrTags.Contains(CurrentTag)) return;
                tag = CurrentTag;
                OrTags.Add(tag);
                RemoveIfExist(tag, NotTags);
                RemoveIfExist(tag, AndTags);
                break;
            case TagSearchStates.Not:
                if (NotTags.Contains(CurrentTag)) return;
                tag = CurrentTag;
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
            list.Remove(tag);
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
}