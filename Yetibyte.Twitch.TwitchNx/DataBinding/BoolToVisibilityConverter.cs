using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Yetibyte.Twitch.TwitchNx.DataBinding
{

    [ValueConversion(typeof(Boolean), typeof(Visibility))]
	public sealed class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool IsInverted = parameter == null ? false : (bool)parameter;
			bool IsVisible = value == null ? false : (bool)value;
			if (IsVisible)
				return IsInverted ? Visibility.Hidden : Visibility.Visible;
			else
				return IsInverted ? Visibility.Visible : Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Visibility visiblility = value == null ? Visibility.Hidden : (Visibility)value;
			bool IsInverted = parameter == null ? false : (bool)parameter;

			return (visiblility == Visibility.Visible) != IsInverted;
		}
	}

	[ValueConversion(typeof(Boolean), typeof(Visibility))]
	public sealed class BoolToCollapsedConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool IsInverted = parameter == null ? false : (bool)parameter;
			bool IsVisible = value == null ? false : (bool)value;
			if (IsVisible)
				return IsInverted ? Visibility.Collapsed : Visibility.Visible;
			else
				return IsInverted ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Visibility visiblility = value == null ? Visibility.Collapsed : (Visibility)value;
			bool IsInverted = parameter == null ? false : (bool)parameter;

			return (visiblility == Visibility.Visible) != IsInverted;
		}
	}
}
