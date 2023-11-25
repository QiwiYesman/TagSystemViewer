using System.Collections.ObjectModel;
using ReactiveUI;
using TagSystemViewer.Enums;
using TagSystemViewer.Models;

namespace TagSystemViewer.ViewModels.Observables;

public class ObservableUrl: ViewModelBase
{
    
    private RecordStates _recordStates;
    private string _currentLink ="";
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
        set =>this.RaiseAndSetIfChanged(ref _recordStates, value);
    }
    public ObservableCollection<Tag> Tags { get; set; }
    
    
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