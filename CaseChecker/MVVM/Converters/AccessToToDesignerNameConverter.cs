using CaseChecker.MVVM.ViewModel;
using System.Globalization;
using System.Windows.Data;

namespace CaseChecker.MVVM.Converters;

public class AccessToToDesignerNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string val)
        {
            if (ManagementViewModel.Instance.DesignersModel.FirstOrDefault(x => x.DesignerID == val) != null)
            {
                return ManagementViewModel.Instance.DesignersModel.FirstOrDefault(x => x.DesignerID == val)!.FriendlyName!;
            }
            else if (val.Equals("both", StringComparison.CurrentCultureIgnoreCase))
            {
                return "Admin";
            }
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
