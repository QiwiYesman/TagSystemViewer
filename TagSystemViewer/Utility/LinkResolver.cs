using System;
using System.IO;

namespace TagSystemViewer.Utility;

public static class LinkResolver
{
    public static bool IsRelative(string uri) => !Path.IsPathRooted(uri);

    public static string Unescape(string uri) => Uri.UnescapeDataString(uri);
    public static string GetFullPath(string absolutePath) => Uri.UnescapeDataString(absolutePath);
    public static string GetFullPath(Uri uri) => Uri.UnescapeDataString(uri.AbsolutePath);

    public static string GetRelativeUrl(string mountPoint, string absolutePath)
    {
        var uriTo= new Uri(absolutePath);
        var uriMount = new Uri(mountPoint);
        return Unescape(uriMount.MakeRelativeUri(uriTo).ToString());
    }

    public static string ResolveAbsoluteUrl(string mountPoint, string relativePath)
    {
        var mountUri = new Uri(mountPoint);
        var combinedUri = new Uri(mountUri, relativePath);
        return GetFullPath(combinedUri);
    }
    
    public static string ResolveAbsoluteUrl(Uri mountUri, string relativePath)
    {
        return GetFullPath(new Uri(mountUri, relativePath));
    }
}