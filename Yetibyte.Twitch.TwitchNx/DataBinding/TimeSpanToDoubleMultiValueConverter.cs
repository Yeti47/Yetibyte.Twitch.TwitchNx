using System;
using System.Globalization;
using System.Windows.Data;

namespace Yetibyte.Twitch.TwitchNx.DataBinding
{
    internal class TimeSpanToDoubleMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan timeSpan = (TimeSpan)values[0];
            float unitsPerSecond = (float)values[1];

            return timeSpan.TotalSeconds * unitsPerSecond;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
