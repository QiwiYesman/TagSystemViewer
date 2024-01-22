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
        Files[FileExtensions.Docx] = "word";
        Files[FileExtensions.Unknown] = "unknown";
        Files[FileExtensions.Txt] = "txt";
        Files[FileExtensions.Xlsx] = "excel";
        Files[FileExtensions.Error] = "no_access";
        Files[FileExtensions.Video] = "video";
        Files[FileExtensions.Audio] = "audio";
        Files[FileExtensions.Web] = "web";
        Files[FileExtensions.Pdf] = "pdf";
    }
    
    public static string GetFileName(FileExtensions ext) =>
        AssetsResolver.Thumbnails + GetFileNameMedium(ext);

    public static string GetFileNameWithoutExtension(FileExtensions ext) =>
        Files[ext] + "_file_";

    public static string GetFileNameMedium(FileExtensions ext) =>
        GetFileNameWithoutExtension(ext) + "_medium" + ".png";
    public static Stream OpenThumbnail(FileExtensions ext)
    {
        var name = GetFileName(ext);
        return AssetLoader.Open(new Uri(name));
    }
}