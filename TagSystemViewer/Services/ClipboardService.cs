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
    public bool IsWindows { get; set; } = false;
 

    public ClipboardService(Window window)
    {
        _window = window;
        IsWindows = OperatingSystem.IsWindows();
    }
    
    public Task SetTextAsync(string? text) => _window.Clipboard?.SetTextAsync(text) ?? Task.CompletedTask;

    public async Task SetFileAsyncLinux(string filePath)
    {
        var clip = _window.Clipboard;
        if (clip is null) return;
        var dataObject = new DataObject();
        dataObject.Set(DataFormats.Files, filePath);
        dataObject.Set("FileDrop", filePath);
        dataObject.Set("text/uri-list", filePath);
        await clip.SetDataObjectAsync(dataObject);
    }
    public async Task SetFileAsyncWindows(string filePath)
    {
        var clip = _window.Clipboard;
        if (clip is null) return;
        var uri = new Uri(filePath);
        var dataObject = new DataObject();
        var file = await _window.StorageProvider.TryGetFileFromPathAsync(uri);
        if (file is null) return;
        dataObject.Set(DataFormats.Files, new List<IStorageFile>{file});
        await clip.SetDataObjectAsync(dataObject);
    }

    public async Task SetFileAsync(string filePath)
    {
        switch (IsWindows)
        {
            case false:
                await SetFileAsyncLinux(filePath);
                break;
            default:
                await SetFileAsyncWindows(filePath);
                break;
        }
    }
}