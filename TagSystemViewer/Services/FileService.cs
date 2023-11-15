using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace TagSystemViewer.Services;

public class FileService
{
    private readonly Window _window;

    public FileService(Window window)
    {
        _window = window;
    }
    
    public async Task<IStorageFile?> OpenFileDialog()
    {
        var files = await _window.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Оберіть файл...",
                AllowMultiple = false
            }
        );
        return files.Count > 0 ? files[0] : null;
    }

    public async Task<IStorageFolder?> OpenFolderDialog()
    {
        var folders = await _window.StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                Title = "Оберіть теку збереження",
                AllowMultiple = false
            }
        );
        return folders.Count > 0 ? folders[0] : null;
    }

    public async Task<IStorageFile?> SaveFileDialog() =>
        await _window.StorageProvider.SaveFilePickerAsync(
            new FilePickerSaveOptions()
            {
                Title = "Вкажіть базу даних для створення"
            }
        );
}