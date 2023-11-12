using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using TagSystemViewer.Enums;
using TagSystemViewer.Models;

namespace TagSystemViewer.Converters;

public static class AssetsResolver
{
    public static string Assembly => "avares://TagSystemViewer/";
    public static string Assets => Assembly + "Assets/";
    public static string Thumbnails => Assets + "FileThumbnails/";
}

public static class ExtensionsNames
{
    public static string[] ImageExtensions => new[] { "png", "jpeg", "jpg", "bmp" };
    public static string[] DocExtensions => new[] { "doc", "docx" };
    public static string[] AudioExtensions => new[] { "mp3", "webp" };
}
public static class TagToThumbnail
{
    private static readonly OrderedDictionary Files = new();
    static TagToThumbnail()
    {
        Files[FileExtensions.Docx] = "docx_file.png";
        Files[FileExtensions.Unknown] = "unknown_file.jpg";
        Files[FileExtensions.Gif] = "unknown_file.jpg";
    }

    public static string GetFileName(FileExtensions ext) =>
       AssetsResolver.Thumbnails + Files[ext];
    
    public static FileExtensions DefineExtension(string tagName) =>
        tagName switch
        {
            "docx" => FileExtensions.Docx,
            "xlsx" => FileExtensions.Xlsx,
            "image" => FileExtensions.Image,
            "gif" => FileExtensions.Gif,
            _ => FileExtensions.Unknown
        };
}
public class TagsToThumbnailsConverter: IValueConverter
{
    public static Bitmap OpenThumbnail(FileExtensions ext) =>
        new(AssetLoader.Open(new Uri(TagToThumbnail.GetFileName(ext))));
    
    
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Url url)
        {
            foreach (var tag in url.Tags)
            {
                switch (tag.Name)
                {
                    case "image":
                    {
                        return url.Link;
                    }
                    case "docx":
                    { 
                        return  TagToThumbnail.GetFileName(FileExtensions.Docx);
                    }
                    case "gif":
                    {
                        return TagToThumbnail.GetFileName(FileExtensions.Gif);
                    }
                }
            }
            
        }
        
        return TagToThumbnail.GetFileName(FileExtensions.Unknown);

    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}