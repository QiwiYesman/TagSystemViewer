using System.Collections.Generic;
using TagSystemViewer.Enums;

namespace TagSystemViewer.Models;

public static class DefaultTagsAndExtensions
{
    private static readonly Dictionary<string, FileExtensions> Dictionary;
    
    static DefaultTagsAndExtensions()
    {
        Dictionary = new Dictionary<string, FileExtensions>
        {
            ["image"] = FileExtensions.Image,
            ["gif"] = FileExtensions.Gif,
            ["docx"] = FileExtensions.Docx,
            ["xls"] = FileExtensions.Xlsx,
            ["pdf"] = FileExtensions.Pdf,
            ["audio"] = FileExtensions.Audio,
            ["video"] = FileExtensions.Video,
            ["web"] = FileExtensions.Web,
            ["txt"] = FileExtensions.Txt
        };
    }

    public static IEnumerable<string> TagNames => Dictionary.Keys;
    public static FileExtensions Get(string tagName) =>
        Dictionary.GetValueOrDefault(tagName, FileExtensions.Unknown);
}