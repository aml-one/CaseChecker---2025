using System.Globalization;
using System.Windows.Data;

namespace CaseChecker.MVVM.Converters;

public class LanguageToFlagConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string lang = "English";
        if (value is string val)
        {
            if (val == "English")
                lang = "English";
            else
                lang = "Chinese";

        }

        return string.Format("/Images/{0}.png", lang);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
