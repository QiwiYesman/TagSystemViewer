using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Platform;

namespace TagSystemViewer.Utility;
public static class UriToAsset
{
    private static readonly Dictionary<string ,string> AssetShortNames = new()
    {
        ["png"]="image",
        ["jpg"]="image",
        ["jpeg"]="image",
        ["bmp"]="image",
        ["webp"]="image",
        ["tiff"]="image",
        ["avif"]="image",
        ["ico"]="image",
        ["docx"]= "word",
        ["doc"] = "word",
        ["xls"] = "excel",
        ["xlsx"] = "excel",
        ["mp4"] = "video",
        ["webm"] = "video",
        ["avi"] = "video",
        ["mp3"] = "audio",
        ["wav"] = "audio",
        ["m4a"] = "audio",
        ["txt"] = "txt",
        ["pdf"] = "pdf",
        ["error"] = "no_access"
    };
    
    public static string GetExtension(string path)
    {
        var extension = Path.GetExtension(path)[1..];
        return extension == string.Empty ? "unknown" : extension;
    }

    public static string GetAssetName(string extension)
    {
        return AssetShortNames.GetValueOrDefault(extension, "unknown");
    }
    

    public static string GetAssetPath(string assetName)
    {
        return AssetsResolver.Thumbnails + assetName + "_file__medium.png";
    }

    public static Stream OpenThumbnailByAssetName(string assetName)
    {
        var name = GetAssetPath(assetName);
        return AssetLoader.Open(new Uri(name));
    }
    public static Stream OpenThumbnailByExtension(string extension)
    {
        var name = GetAssetPath(GetAssetName(extension));
        return AssetLoader.Open(new Uri(name));
    }
}