using System.Globalization;
using System.Windows.Data;

namespace CaseChecker.MVVM.Converters;

public class RemoveFirstCharFromStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string val) 
        {
            return val[1..];
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
