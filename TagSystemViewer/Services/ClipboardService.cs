using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;

namespace TagSystemViewer.Services;

public class ClipboardService
{
    private readonly Window _window;
    public string ContentDataFormat { get; set; } = "text/uri-list";

    public ClipboardService(Window window)
    {
        _window = window;
    }
    
    public Task SetTextAsync(string? text) => _window.Clipboard?.SetTextAsync(text) ?? Task.CompletedTask;

    public async Task SetFileAsync(string filePath)
    {
        var clip = _window.Clipboard;
        if (clip is null) return;
        var uri = new Uri(filePath);
        var dataObject = new DataObject();
        var file = await _window.StorageProvider.TryGetFileFromPathAsync(uri);
        if (file is null) return;
        dataObject.Set(ContentDataFormat, new List<IStorageFile>{file});
        await clip.SetDataObjectAsync(dataObject);
    }
}