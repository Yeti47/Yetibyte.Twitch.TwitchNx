using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.Models;

namespace Yetibyte.Twitch.TwitchNx.DataBinding
{
    [ValueConversion(typeof(ControllerType), typeof(ImageSource))]
    internal class ControllerTypeToImageSourceConverter : IValueConverter
    {
        private const string PRO_CONTROLLER_IMAGE_PATH = "/Images/procon.png";
        private const string JOYCON_LEFT_IMAGE_PATH = "/Images/joyconl.png";
        private const string JOYCON_RIGHT_IMAGE_PATH = "/Images/joyconr.png";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(ImageSource))
                return Binding.DoNothing;

            ControllerType controllerType = (ControllerType)value;

            string imagePath = controllerType switch
            {
                ControllerType.JoyConLeft => JOYCON_LEFT_IMAGE_PATH,
                ControllerType.JoyConRight => JOYCON_RIGHT_IMAGE_PATH,
                _ => PRO_CONTROLLER_IMAGE_PATH
            };

            return new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
