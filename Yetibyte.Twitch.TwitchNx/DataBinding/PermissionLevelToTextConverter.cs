using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;

namespace Yetibyte.Twitch.TwitchNx.DataBinding
{
    [ValueConversion(typeof(PermissionLevel), typeof(string))]
    internal class PermissionLevelToTextConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty AnyTextProperty =
             DependencyProperty.Register(nameof(AnyText), typeof(string),
             typeof(PermissionLevelToTextConverter));

        public string AnyText
        {
            get { return (string)GetValue(AnyTextProperty); }
            set { SetValue(AnyTextProperty, value); }
        }

        public static readonly DependencyProperty SubTextProperty =
             DependencyProperty.Register(nameof(SubText), typeof(string),
             typeof(PermissionLevelToTextConverter));

        public string SubText
        {
            get { return (string)GetValue(SubTextProperty); }
            set { SetValue(SubTextProperty, value); }
        }

        public static readonly DependencyProperty ModTextProperty =
             DependencyProperty.Register(nameof(ModText), typeof(string),
             typeof(PermissionLevelToTextConverter));

        public string ModText
        {
            get { return (string)GetValue(ModTextProperty); }
            set { SetValue(ModTextProperty, value); }
        }

        public static readonly DependencyProperty OwnTextProperty =
             DependencyProperty.Register(nameof(OwnText), typeof(string),
             typeof(PermissionLevelToTextConverter));

        public string OwnText
        {
            get { return (string)GetValue(OwnTextProperty); }
            set { SetValue(OwnTextProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                return Binding.DoNothing;

            PermissionLevel permissionLevel = (PermissionLevel)value;

            string text = permissionLevel switch
            {
                PermissionLevel.Sub => SubText,
                PermissionLevel.Mod => ModText,
                PermissionLevel.Own => OwnText,
                _ => AnyText
            };

            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(PermissionLevel))
                return Binding.DoNothing;

            string text = value.ToString() ?? string.Empty;

            if (text == SubText)
                return PermissionLevel.Sub;

            if (text == ModText)
                return PermissionLevel.Mod;

            if (text == OwnText)
                return PermissionLevel.Own;

            return PermissionLevel.Any;
        }
    }
}
