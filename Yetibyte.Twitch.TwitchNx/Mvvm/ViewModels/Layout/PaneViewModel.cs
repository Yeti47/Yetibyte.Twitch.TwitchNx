using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout
{
    public class PaneViewModel : ObservableObject
    {
		#region Fields

		private string _title = string.Empty;
		private string _contentId = string.Empty;

		private bool _isSelected = false;
		private bool _isActive = false;

		#endregion Fields

		#region Props

		public string Title
		{
			get => _title;
			set
			{
				if (_title != value)
				{
					_title = value;
					OnPropertyChanged();
				}
			}
		}

		public ImageSource? IconSource { get; protected set; }

		public string ContentId
		{
			get => _contentId;
			set
			{
				if (_contentId != value)
				{
					_contentId = value;
					OnPropertyChanged();
				}
			}
		}

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					OnPropertyChanged(); 
				}
			}
		}

		public bool IsActive
		{
			get => _isActive;
			set
			{
				if (_isActive != value)
				{
					_isActive = value;
					OnPropertyChanged();
				}
			}
		}

        #endregion Props

        #region Ctors

        public PaneViewModel()
        {

        }

        #endregion
    }
}
