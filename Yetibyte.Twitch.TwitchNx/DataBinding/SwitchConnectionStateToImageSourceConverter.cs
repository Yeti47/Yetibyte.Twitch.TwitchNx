using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Yetibyte.Twitch.TwitchNx.Mvvm.Models;

namespace Yetibyte.Twitch.TwitchNx.DataBinding
{
    [ValueConversion(typeof(SwitchConnectionState), typeof(ImageSource))]
    internal class SwitchConnectionStateToImageSourceConverter : IValueConverter
    {
        private const string DISCONNECTED_IMAGE_PATH = "/Images/Computer_lab_icon.png";
        private const string CONNECTED_TO_BRIDGE_IMAGE_PATH = "/Images/bridge-31.png";
        private const string CONNECTED_TO_SWITCH_IMAGE_PATH = "/Images/Nintendo_Switch_logo.png";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(ImageSource))
                return Binding.DoNothing;

            SwitchConnectionState switchConnectionState = (SwitchConnectionState)value;

            string imagePath = switchConnectionState switch
            {
                SwitchConnectionState.ConnectedToBridge => CONNECTED_TO_BRIDGE_IMAGE_PATH,
                SwitchConnectionState.ConnectedToSwitch => CONNECTED_TO_SWITCH_IMAGE_PATH,
                _ => DISCONNECTED_IMAGE_PATH
            };

            return new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
