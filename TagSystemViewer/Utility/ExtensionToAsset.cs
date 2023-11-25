using System;
using System.Collections.Specialized;
using System.IO;
using Avalonia.Platform;
using TagSystemViewer.Enums;

namespace TagSystemViewer.Utility;
public static class ExtensionToAsset
{
    private static readonly OrderedDictionary Files = new();
    static ExtensionToAsset()
    {
        Files[FileExtensions.Docx] = "word.png";
        Files[FileExtensions.Unknown] = "unknown.png";
        Files[FileExtensions.Txt] = "txt.png";
        Files[FileExtensions.Xlsx] = "excel.png";
        Files[FileExtensions.Error] = "no_access.png";
        Files[FileExtensions.Video] = "video.png";
        Files[FileExtensions.Audio] = "audio.png";
        Files[FileExtensions.Web] = "web.png";
        Files[FileExtensions.Pdf] = "pdf.png";
    }
    
    public static string GetFileName(FileExtensions ext) =>
        AssetsResolver.Thumbnails + Files[ext];

    public static Stream OpenThumbnail(FileExtensions ext)
    {
        var name = GetFileName(ext);
        return AssetLoader.Open(new Uri(name));
    }
}