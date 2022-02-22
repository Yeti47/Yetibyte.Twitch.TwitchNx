using System;
using System.Globalization;
using System.Windows.Data;

namespace Yetibyte.Twitch.TwitchNx.DataBinding
{
    [ValueConversion(typeof(bool), typeof(bool))]
	public sealed class InverseBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}
	}
}
