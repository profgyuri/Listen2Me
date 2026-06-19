using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Listen2Me.WPF;

/// <summary>
/// Checks if the path is equal to the parameter to determine if the navbar item should be selected.
/// </summary>
public class PathComparer : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value?.ToString() == parameter?.ToString();

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Convert true to visible and false to collapsed.
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
            throw new InvalidOperationException("Value must be a boolean.");

        if (boolValue)
        {
            return Visibility.Visible;
        }
        
        return Visibility.Collapsed;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}