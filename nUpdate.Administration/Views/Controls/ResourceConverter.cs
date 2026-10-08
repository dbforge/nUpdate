using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace nUpdate.Administration.Views.Controls;

/// <summary>Looks up an application resource by the key a view model names, such as the icon of an operation kind.</summary>
public sealed class ResourceConverter : IValueConverter
{
    public static ResourceConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is string key && Application.Current!.TryGetResource(key, Application.Current.ActualThemeVariant, out var resource)
            ? resource
            : null;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
