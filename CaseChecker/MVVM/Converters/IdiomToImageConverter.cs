using System.Globalization;
using System.Windows.Data;

namespace CaseChecker.MVVM.Converters;

public class IdiomToImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string val)
        {
            return @$"\Images\{val}.png";
        }

        return @$"\Images\Unknown.png";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
