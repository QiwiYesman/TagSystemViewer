using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace TagSystemViewer.Services;

public class ClipboardService
{
    private readonly Window _window;

    public ClipboardService(Window window)
    {
        _window = window;
    }

    private IClipboard? Clipboard => _clipboard ??= _window.Clipboard;
    private IClipboard? _clipboard;

    public Task<string?> GetTextAsync() => Clipboard?.GetTextAsync() ?? Task.FromResult<string?>(null);
    public Task SetTextAsync(string? text) => Clipboard?.SetTextAsync(text) ?? Task.CompletedTask;
    public Task ClearAsync() => Clipboard?.ClearAsync() ?? Task.CompletedTask;
    public Task SetDataObjectAsync(IDataObject data) => Clipboard?.SetDataObjectAsync(data) ?? Task.CompletedTask;
}