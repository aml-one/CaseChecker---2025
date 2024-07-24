using System.Globalization;
using System.Windows.Data;

namespace CaseChecker.MVVM.Converters;

public class StringToUriConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string lang = "English";
        if (value is string val)
        {
            if (val == "English")
                lang = "Chinese";
            else
                lang = "English";

        }

        return string.Format("/Images/{0}.png", lang);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
