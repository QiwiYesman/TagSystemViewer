using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;

namespace TagSystemViewer.Converters;

public class UrlToShortNameConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string) return "";
        
        var link = value.ToString();
        if (link is null) return "";
        if (!File.Exists(link)) return Uri.UnescapeDataString(link);
        var uri = new Uri(link);
        return Uri.UnescapeDataString(uri.Segments[^1]);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}