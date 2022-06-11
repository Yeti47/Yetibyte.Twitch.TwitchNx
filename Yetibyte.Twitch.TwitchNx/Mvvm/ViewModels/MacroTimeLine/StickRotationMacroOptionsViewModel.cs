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
        public record StickDirectionViewModel(ControllerStickDirection ControllerStickDirection, string Name);

        private readonly List<StickDirectionViewModel> _controllerStickDirections = new List<StickDirectionViewModel>()
        {
            new StickDirectionViewModel(ControllerStickDirection.North, "North (0°)"),
            new StickDirectionViewModel(ControllerStickDirection.NorthEast, "North East (45°)"),
            new StickDirectionViewModel(ControllerStickDirection.East, "East (90°)"),
            new StickDirectionViewModel(ControllerStickDirection.SouthEast, "South East (135°)"),
            new StickDirectionViewModel(ControllerStickDirection.South, "South (180°)"),
            new StickDirectionViewModel(ControllerStickDirection.SouthWest, "South West (225°)"),
            new StickDirectionViewModel(ControllerStickDirection.West, "West (270°)"),
            new StickDirectionViewModel(ControllerStickDirection.NorthWest, "North West (315°)"),
        };

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

        public IEnumerable<StickDirectionViewModel> StickDirections => _controllerStickDirections;

        public StickRotationMacroOptionsViewModel()
        {
            _selectedStartDirection = _controllerStickDirections.First();
            _selectedEndDirection = _controllerStickDirections.First();
        }

        public StickRotationMacroOptionsViewModel(ControllerStickDirection startDirection, ControllerStickDirection endDirection, bool isCcw = false)
        {
            _selectedStartDirection = _controllerStickDirections.First(c => c.ControllerStickDirection == startDirection);
            _selectedEndDirection = _controllerStickDirections.First(c => c.ControllerStickDirection == endDirection);
            _isCounterClockwise = isCcw;
        }

    }
}
