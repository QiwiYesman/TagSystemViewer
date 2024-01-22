using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using TagSystemViewer.Enums;

namespace TagSystemViewer.Converters;

public class RecordStateToClassesConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is RecordStates state)
        {
            return state switch
            {
                RecordStates.Delete => new Classes("Exclude"),
                RecordStates.Insert => new Classes("Add"),
                RecordStates.Update => new Classes("Update"),
                RecordStates.UpdateAndMove => new Classes("MoveFile"),
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