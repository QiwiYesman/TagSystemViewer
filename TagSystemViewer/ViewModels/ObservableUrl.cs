using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using ReactiveUI;
using TagSystemViewer.Enums;
using TagSystemViewer.Models;

namespace TagSystemViewer.ViewModels;

public class ObservableUrl: ViewModelBase
{
    private static readonly OrderedDictionary RecordStatesNames = new()
    {
        {RecordStates.NotModified, "Не змінюється"},
        {RecordStates.Insert, "Для додання"},
        {RecordStates.Update, "Для оновлення"},
        {RecordStates.Delete, "Для видалення"},
    };

    private RecordStates _recordStates;
    private string _currentLink;
    public int Id { get; set; }

    public string CurrentLink
    {
        get => _currentLink;
        set => this.RaiseAndSetIfChanged(ref _currentLink, value);
    }
    public string? OldName { get; set; }
    public RecordStates RecordStates
    {
        get => _recordStates;
        set
        {
            _recordStates = value;
            this.RaisePropertyChanged(nameof(RecordStateName));
        }
        
    }
    public ObservableCollection<Tag> Tags { get; set; }

    public string RecordStateName => RecordStatesNames[RecordStates]?.ToString() ?? "";
    
    public ObservableUrl(Url url)
    {
        Id = url.Id;
        CurrentLink = url.Link;
        Tags = new ObservableCollection<Tag>(url.Tags);
        RecordStates = RecordStates.NotModified;
    }

    public ObservableUrl(string link)
    {
        CurrentLink = link;
        Id = 0;
        RecordStates = RecordStates.Insert;
        Tags = new();
    }

    public override string ToString() => CurrentLink;
}