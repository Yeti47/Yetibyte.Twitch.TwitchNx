using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Yetibyte.Twitch.TwitchNx.DataBinding
{
    [ValueConversion(typeof(bool), typeof(string))]
    internal class BoolToTextConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty TrueTextProperty =
             DependencyProperty.Register(nameof(TrueText), typeof(string),
             typeof(BoolToTextConverter));

        public static readonly DependencyProperty FalseTextProperty =
             DependencyProperty.Register(nameof(FalseText), typeof(string),
             typeof(BoolToTextConverter));


        public string TrueText
        {
            get { return (string)GetValue(TrueTextProperty); }
            set { SetValue(TrueTextProperty, value); }
        }

        public string FalseText
        {
            get { return (string)GetValue(FalseTextProperty); }
            set { SetValue(FalseTextProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (targetType != typeof(string))
            //    throw new ArgumentException($"The target type must be {nameof(String)}.");

            bool boolValue = (bool)value;

            return boolValue ? TrueText : FalseText;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
