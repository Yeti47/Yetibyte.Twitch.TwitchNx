using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroTimeLine
{
    public class FixedStickDirectionMacroOptionsViewModel : ObservableObject
    {
        private const byte MIN_PRESSURE = 0;
        private const byte MAX_PRESSURE = 100;

        private StickDirectionViewModel _selectedDirection;
        private byte _pressure = MAX_PRESSURE;

        public StickDirectionViewModel SelectedDirection
        {
            get { return _selectedDirection; }
            set
            {
                _selectedDirection = value;
                OnPropertyChanged();
            }
        }

        public byte Pressure
        {
            get { return _pressure; }
            set
            {
                _pressure = Math.Min(Math.Max(MIN_PRESSURE, value), MAX_PRESSURE);

                OnPropertyChanged();
            }
        }

        public float NormalizedPressure => (float)_pressure / MAX_PRESSURE;

        public byte MinPressure => MIN_PRESSURE;
        public byte MaxPressure => MAX_PRESSURE;

        public IEnumerable<StickDirectionViewModel> StickDirections => StickDirectionViewModel.All;

        public FixedStickDirectionMacroOptionsViewModel()
        {
            _selectedDirection = StickDirections.First();
        }

        public FixedStickDirectionMacroOptionsViewModel(ControllerStickDirection controllerStickDirection, float normalizedPressure = 1f)
        {
            _selectedDirection = StickDirections.First(d => d.ControllerStickDirection == controllerStickDirection);
            Pressure = (byte)(MIN_PRESSURE + normalizedPressure * MAX_PRESSURE);
        }


    }
}
