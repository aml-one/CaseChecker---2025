using System.Globalization;
using System.Windows.Data;

namespace CaseChecker.MVVM.Converters;

public class LeftSideColumnSpanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int val)
        {
            if (val == 2 || val == 3)
                return 1;
        }
        return 2;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
