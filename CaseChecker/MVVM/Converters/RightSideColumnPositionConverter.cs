using System.Globalization;
using System.Windows.Data;

namespace CaseChecker.MVVM.Converters;

public class RightSideColumnPositionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int val)
        {
            if (val == 2)
                return 0;
        }
        return 1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
