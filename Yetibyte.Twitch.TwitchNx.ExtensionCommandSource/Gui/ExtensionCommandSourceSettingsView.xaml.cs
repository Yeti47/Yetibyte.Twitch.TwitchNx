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
using Yetibyte.Twitch.TwitchNx.CommandSourceGui;

namespace Yetibyte.Twitch.TwitchNx.ExtensionCommandSource.Gui
{
    /// <summary>
    /// Interaction logic for ExtensionCommandSourceSettingsView.xaml
    /// </summary>
    [CommandSourceSettingsView(typeof(ExtensionCommandSourceSettingsViewModel))]
    public partial class ExtensionCommandSourceSettingsView : UserControl
    {
        public ExtensionCommandSourceSettingsView()
        {
            InitializeComponent();
        }
    }
}
