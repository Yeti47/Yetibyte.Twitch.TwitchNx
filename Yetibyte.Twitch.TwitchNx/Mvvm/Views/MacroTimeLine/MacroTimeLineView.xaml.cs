using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.Views.MacroTimeLine
{
    /// <summary>
    /// Interaction logic for MacroTimeLineView.xaml
    /// </summary>
    public partial class MacroTimeLineView : UserControl
    {
        public MacroTimeLineView()
        {
            InitializeComponent();
        }
        protected override AutomationPeer OnCreateAutomationPeer()
        {
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
