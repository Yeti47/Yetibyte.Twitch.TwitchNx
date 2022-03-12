using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Yetibyte.Twitch.TwitchNx.DataBinding
{
    internal class TimeSpanToThicknessMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan timeSpan = (TimeSpan)values[0];
            float unitsPerSecond = (float)values[1];

            Thickness thickness = new Thickness(timeSpan.TotalSeconds * unitsPerSecond, 0, 0, 0);

            return thickness;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
