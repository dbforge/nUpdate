using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace nUpdate.Administration.Infrastructure.Converters
{
    public class GuidToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Guid ? Equals(value, parameter) : DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b)
                return parameter;
            return DependencyProperty.UnsetValue;
        }
    }
}