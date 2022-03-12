using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Yetibyte.Twitch.TwitchNx.DataBinding
{
    [ValueConversion(typeof(string), typeof(ImageSource))]
    internal class StringToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value?.ToString() ?? string.Empty;

            path = path.Replace('/', System.IO.Path.DirectorySeparatorChar);

            path = System.IO.Path.Combine(Environment.CurrentDirectory, path);

            var image = new BitmapImage()
            {
                CacheOption = BitmapCacheOption.OnDemand
            };

            image.BeginInit();
            image.UriSource = new Uri(path, UriKind.Relative);
            image.EndInit();

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
