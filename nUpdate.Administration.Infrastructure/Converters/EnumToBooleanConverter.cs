// EnumToBooleanConverter.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace nUpdate.Administration.Infrastructure.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.GetType().IsEnum)
                return Equals(value, parameter);
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b)
                return parameter;
            return DependencyProperty.UnsetValue;
        }
    }
}