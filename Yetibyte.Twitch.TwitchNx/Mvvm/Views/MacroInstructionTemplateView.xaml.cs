using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.Views
{
    /// <summary>
    /// Interaction logic for MacroInstructionTemplateView.xaml
    /// </summary>
    public partial class MacroInstructionTemplateView : UserControl
    {
        private static readonly Dictionary<string, BitmapImage> _imageCache = new Dictionary<string, BitmapImage>();

        public MacroInstructionTemplateView()
        {
            InitializeComponent();
            DataContextChanged += MacroInstructionTemplateView_DataContextChanged;
        }

        private void MacroInstructionTemplateView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is MacroInstructionTemplateViewModel newVm)
            {
                if (newVm != e.OldValue)
                {
                    int i = 0;

                    imageSourceAnimation.KeyFrames.Clear();
                    imageSourceAnimation.Duration = new Duration(TimeSpan.FromSeconds(newVm.AnimationDuration));

                    foreach (var animationFrame in newVm.AnimationFrames)
                    {
                        Uri imageUri = new Uri(animationFrame.ImagePath, UriKind.Relative);

                        BitmapImage? image;

                        if (!_imageCache.TryGetValue(imageUri.ToString(), out image))
                        {
                            image = new BitmapImage(imageUri) { CacheOption = BitmapCacheOption.Default };
                            _imageCache.Add(imageUri.ToString(), image);
                        }

                        imageSourceAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame()
                            {
                                Value = image,
                                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(newVm.AnimationFrames.Take(i).Sum(f => f.Duration)))   
                            }
                        );

                        i++;
                    }
                }

            }
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            imgIconStatic.Visibility = Visibility.Hidden;
            imgIcon.Visibility = Visibility.Visible;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            imgIcon.Visibility = Visibility.Hidden;
            imgIconStatic.Visibility = Visibility.Visible;
        }
    }


}
