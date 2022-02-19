using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;

namespace Yetibyte.Twitch.TwitchNx.Styling
{
    public class PanesStyleSelector : StyleSelector
    {
		public Style? ToolStyle
		{
			get;
			set;
		}

		public override System.Windows.Style? SelectStyle(object item, System.Windows.DependencyObject container)
		{
			if (item is ToolViewModel)
				return ToolStyle;

			return base.SelectStyle(item, container);
		}
	}
}
