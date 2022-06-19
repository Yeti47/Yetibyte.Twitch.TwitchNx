using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroTimeLine
{
    public class StickRotationMacroOptionsViewModel : ObservableObject
    {
        private StickDirectionViewModel _selectedStartDirection;
        private StickDirectionViewModel _selectedEndDirection;

        private bool _isCounterClockwise = false;

        public StickDirectionViewModel SelectedStartDirection
        {
            get { return _selectedStartDirection; }
            set
            {
                _selectedStartDirection = value;
                OnPropertyChanged();
            }
        }

        public StickDirectionViewModel SelectedEndDirection
        {
            get { return _selectedEndDirection; }
            set
            {
                _selectedEndDirection = value;
                OnPropertyChanged();
            }
        }

        public bool IsCounterClockwise
        {
            get { return _isCounterClockwise; }
            set
            {
                _isCounterClockwise = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<StickDirectionViewModel> StickDirections => StickDirectionViewModel.All;

        public StickRotationMacroOptionsViewModel()
        {
            _selectedStartDirection = StickDirections.First();
            _selectedEndDirection = StickDirections.First();
        }

        public StickRotationMacroOptionsViewModel(ControllerStickDirection startDirection, ControllerStickDirection endDirection, bool isCcw = false)
        {
            _selectedStartDirection = StickDirections.First(c => c.ControllerStickDirection == startDirection);
            _selectedEndDirection = StickDirections.First(c => c.ControllerStickDirection == endDirection);
            _isCounterClockwise = isCcw;
        }

    }
}
