﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CaseChecker.MVVM.Converters;

public class EmptyToCollapsedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string val)
        {
            if (!string.IsNullOrEmpty(val))
                return Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
