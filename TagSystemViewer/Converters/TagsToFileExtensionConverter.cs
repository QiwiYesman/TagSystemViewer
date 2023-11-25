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
using TagSystemViewer.Utility;

namespace TagSystemViewer.Converters;

public class TagsToFileExtensionConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IEnumerable<Tag> tags) return FileExtensions.Unknown;
        foreach (var tag in tags)
        {
            var ext = DefaultTagsAndExtensions.Get(tag.Name);
            if(ext == FileExtensions.Unknown) continue;
            return ext;
        }
        return FileExtensions.Unknown;

    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}