using System;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace TagSystemViewer.Utility;

public static class AsyncLauncher
{
    public static async Task LaunchTask(Action action) => await Task.Run(action);
    public static async Task<T> LaunchTask<T>(Func<T> action) => await Task.Run(action);
    
    public static async void LaunchTaskVoid(Action action) => await Task.Run(action);
    public static async Task LaunchDispatcher(Action action) => await Dispatcher.UIThread.InvokeAsync(action);
    public static async void LaunchDispatcherVoid(Action action) => await Dispatcher.UIThread.InvokeAsync(action);
}