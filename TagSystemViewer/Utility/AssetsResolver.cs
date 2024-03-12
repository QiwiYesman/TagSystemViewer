namespace TagSystemViewer.Utility;

public static class AssetsResolver
{
    public static string Assembly => "avares://TagSystemViewer/";
    public static string Assets => Assembly + "Assets/";
    public static string Thumbnails => Assets + "FileThumbnails/";
    public static string Icons => Assets + "Icons/";

    public static string GetThumbnail(string thumbnailName) => Thumbnails + thumbnailName;

}