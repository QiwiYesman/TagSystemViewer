using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using TagSystemViewer.Enums;

namespace TagSystemViewer.Converters;

public class RecordStateToClassesConverter: IValueConverter
{
    public static Classes CreateClass(string name)
    {
        return new Classes(name);
    }
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is RecordStates state)
        {
            return state switch
            {
                RecordStates.Delete => new Classes("Exclude"),
                RecordStates.Insert => new Classes("Add"),
                RecordStates.Update => new Classes("Update"),
                _ => new Classes()
            };
        }
        
        return new Classes();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}