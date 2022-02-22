using System;
using System.Globalization;
using System.Windows.Data;

namespace Yetibyte.Twitch.TwitchNx.DataBinding
{
    [ValueConversion(typeof(double), typeof(double))]
	public sealed class MinusConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Math.Max(0, System.Convert.ToDouble(value) - System.Convert.ToDouble(parameter, culture));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}
