using System;
using System.Globalization;
using System.Windows.Data;

namespace Yetibyte.Twitch.TwitchNx.DataBinding
{
    [ValueConversion(typeof(double), typeof(double))]
	public sealed class PercentageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter, culture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}
