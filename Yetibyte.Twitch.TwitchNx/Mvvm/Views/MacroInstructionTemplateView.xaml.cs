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
using XamlAnimatedGif;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.Views
{
    /// <summary>
    /// Interaction logic for MacroInstructionTemplateView.xaml
    /// </summary>
    public partial class MacroInstructionTemplateView : UserControl
    {
        public MacroInstructionTemplateView()
        {
            InitializeComponent();
            DataContextChanged += MacroToolBoxItemView_DataContextChanged;
        }

        private void MacroToolBoxItemView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is MacroInstructionTemplateViewModel oldVm)
            {
                oldVm.PropertyChanged -= ViewModel_PropertyChanged;
            }
            if (e.NewValue is MacroInstructionTemplateViewModel newVm)
            {
                newVm.PropertyChanged += ViewModel_PropertyChanged;
            }
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is MacroInstructionTemplateViewModel vm && e.PropertyName == nameof(MacroInstructionTemplateViewModel.IsAnimationPlaying))
            {
                var animator = AnimationBehavior.GetAnimator(imgIcon);

                if (animator is not null && animator.FrameCount > 0)
                {
                    animator.Rewind();

                    if (vm.IsAnimationPlaying)
                    {
                        animator.Play();
                    }
                    else
                    {
                        animator.Pause();
                    }

                }
            }


        }
    }


}
