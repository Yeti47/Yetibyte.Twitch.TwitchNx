using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Yetibyte.Twitch.TwitchNx.Mvvm.Models;

namespace Yetibyte.Twitch.TwitchNx.DataBinding
{
    [ValueConversion(typeof(SwitchConnectionState), typeof(Brush))]
    internal class SwitchConnectionStateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush))
                return Binding.DoNothing;

            SwitchConnectionState switchConnectionState = (SwitchConnectionState)value;

            return switchConnectionState switch
            {
                SwitchConnectionState.ConnectedToBridge => new SolidColorBrush(Color.FromRgb(System.Drawing.Color.Yellow.R, System.Drawing.Color.Yellow.G, System.Drawing.Color.Yellow.B)),
                SwitchConnectionState.ConnectedToSwitch => new SolidColorBrush(Color.FromRgb(System.Drawing.Color.Green.R, System.Drawing.Color.Green.G, System.Drawing.Color.Green.B)),
                _ => new SolidColorBrush(Color.FromRgb(System.Drawing.Color.Red.R, System.Drawing.Color.Red.G, System.Drawing.Color.Red.B))
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
