using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Yetibyte.Twitch.TwitchNx.DataBinding
{
    [ValueConversion(typeof(Boolean), typeof(Brush))]
    public class BooleanToBrushConverter : DependencyObject, IValueConverter
    {
        public SolidColorBrush FalseBrush
        {
            get { return (SolidColorBrush)GetValue(FalseBrushProperty); }
            set { SetValue(FalseBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FalseBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FalseBrushProperty =
            DependencyProperty.Register("FalseBrush", typeof(SolidColorBrush), typeof(BooleanToBrushConverter));

        public SolidColorBrush TrueBrush
        {
            get { return (SolidColorBrush)GetValue(TrueBrushProperty); }
            set { SetValue(TrueBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TrueBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrueBrushProperty =
            DependencyProperty.Register("TrueBrush", typeof(SolidColorBrush), typeof(BooleanToBrushConverter));


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool booleanValue = (bool)value;

            return booleanValue ? TrueBrush : FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
