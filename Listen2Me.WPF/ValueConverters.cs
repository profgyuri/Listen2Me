using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

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

/// <summary>
/// Convert ConnectionState to color.
/// </summary>
public class DbConnectionStateToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ConnectionState state)
            throw new InvalidOperationException("Value must be a ConnectionState.");

        switch (state)
        {
            case ConnectionState.Broken:
                return new SolidColorBrush(Color.FromRgb(255, 0, 0));
            case ConnectionState.Closed:
                return new SolidColorBrush(Color.FromRgb(128, 128, 128));
            case ConnectionState.Connecting:
                return new SolidColorBrush(Color.FromRgb(255, 255, 0));
            case ConnectionState.Open:
                return new SolidColorBrush(Color.FromRgb(0, 128, 0));
            default:
                return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}