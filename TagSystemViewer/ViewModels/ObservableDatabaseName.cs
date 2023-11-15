using ReactiveUI;

namespace TagSystemViewer.ViewModels;

public class ObservableDatabaseName :ViewModelBase
{
    private string _name="", _path="";

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    public string Path
    {
        get => _path;
        set => this.RaiseAndSetIfChanged(ref _path, value);
    }
}