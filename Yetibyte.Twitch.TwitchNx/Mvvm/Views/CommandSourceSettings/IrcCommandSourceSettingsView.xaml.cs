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
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.Views.CommandSourceSettings
{
    /// <summary>
    /// Interaction logic for IrcCommandSourceSettingsView.xaml
    /// </summary>
    [CommandSourcePlugin.CommandSourceSettingsView(typeof(IrcCommandSourceSettingsViewModel))]
    public partial class IrcCommandSourceSettingsView : UserControl
    {
        public IrcCommandSourceSettingsView()
        {
            InitializeComponent();
        }
    }
}
