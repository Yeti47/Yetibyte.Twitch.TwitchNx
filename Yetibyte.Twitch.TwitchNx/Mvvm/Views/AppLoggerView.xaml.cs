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

namespace Yetibyte.Twitch.TwitchNx.Mvvm.Views
{
    /// <summary>
    /// Interaction logic for AppLoggerView.xaml
    /// </summary>
    public partial class AppLoggerView : UserControl
    {
        private bool _isAutoScrollEnabled = false;

        public AppLoggerView()
        {
            InitializeComponent();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is not ScrollViewer scrollViewer)
                return;

            if (e.ExtentHeightChange == 0)
                _isAutoScrollEnabled = scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight;
            else if (_isAutoScrollEnabled)
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
        }
    }
}
