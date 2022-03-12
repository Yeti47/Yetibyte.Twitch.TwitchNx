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
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroTimeLine;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.Views.MacroTimeLine
{
    /// <summary>
    /// Interaction logic for MacroTimeTrackElementView.xaml
    /// </summary>
    public partial class MacroTimeTrackElementView : UserControl
    {
        public MacroTimeTrackElementView()
        {
            InitializeComponent();
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (DataContext is MacroTimeTrackElementViewModel vm)
            {
                vm.AdjustDurationByUnits(e.HorizontalChange);
            }
        }

        private void Thumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            StartDrag(sender);
        }

        private void Thumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            EndDrag();
        }

        private void EndDrag()
        {
            if (DataContext is MacroTimeTrackElementViewModel vm)
            {
                vm.IsDraggable = true;
            }
        }

        private void ThumbLeft_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (DataContext is MacroTimeTrackElementViewModel vm)
            {
                vm.AdjustStartTimeByUnits(e.HorizontalChange);
            }
        }

        private void ThumbLeft_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            StartDrag(sender);
        }

        private void StartDrag(object sender)
        {

            if (Mouse.Capture((IInputElement)sender) && DataContext is MacroTimeTrackElementViewModel vm)
            {
                vm.IsDraggable = false;

            }
        }

        private void ThumbLeft_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            EndDrag();
        }

    }
}
