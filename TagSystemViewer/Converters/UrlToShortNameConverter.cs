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
        return string.IsNullOrEmpty(link) ? 
            "" : Path.GetFileName(Uri.UnescapeDataString(link));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}