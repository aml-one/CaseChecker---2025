using System.Globalization;
using System.Windows.Data;

namespace CaseChecker.MVVM.Converters;

public class DateFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string val)
        {
            if (DateTime.TryParse(val, out DateTime date))
                return date.ToString("MMM d - h:mm:ss tt");
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
