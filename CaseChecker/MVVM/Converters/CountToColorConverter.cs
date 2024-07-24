using System.Globalization;
using System.Windows.Data;

namespace CaseChecker.MVVM.Converters;

public class CountToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int val)
        {
            if (val > 1)
                return "Pink";
        }
        return "LightGreen";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return 1;
    }
}
