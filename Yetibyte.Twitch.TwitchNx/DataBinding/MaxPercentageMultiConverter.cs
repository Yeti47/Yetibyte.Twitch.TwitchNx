using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Yetibyte.Twitch.TwitchNx.DataBinding
{
    public class MaxPercentageMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || !values.All(v => v is double))
                return Binding.DoNothing;

            return Math.Max(System.Convert.ToDouble(values[0]), System.Convert.ToDouble(values[1]))
                * System.Convert.ToDouble(parameter, culture) / 100.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
