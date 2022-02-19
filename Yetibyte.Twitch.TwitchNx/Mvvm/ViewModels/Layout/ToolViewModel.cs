using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout
{
    public class ToolViewModel : PaneViewModel
    {
		#region Fields

		private bool _isVisible = true;

		#endregion Fields

		#region Props

		public string Name { get; private set; } = string.Empty;

		public bool IsVisible
		{
			get => _isVisible;
			set
			{
				if (_isVisible != value)
				{
					_isVisible = value;
					OnPropertyChanged();
				}
			}
		}

        #endregion Props

        #region Ctors

        public ToolViewModel(string name)
        {
			Title = Name = name;
        }

        #endregion
    }
}
