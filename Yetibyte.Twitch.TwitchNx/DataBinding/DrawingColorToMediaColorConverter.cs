using System;
using System.Globalization;
using System.Windows.Data;

namespace Yetibyte.Twitch.TwitchNx.DataBinding
{
    [ValueConversion(typeof(System.Drawing.Color), typeof(System.Windows.Media.Color))]
    public class DrawingColorToMediaColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Drawing.Color color = (System.Drawing.Color)value;

            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Windows.Media.Color color = (System.Windows.Media.Color)value;

            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
