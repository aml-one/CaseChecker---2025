﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CaseChecker.MVVM.Converters;

public class RightSideColumnVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int val)
        {
            if (val == 2 || val == 3)
                return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return 0;
    }
}