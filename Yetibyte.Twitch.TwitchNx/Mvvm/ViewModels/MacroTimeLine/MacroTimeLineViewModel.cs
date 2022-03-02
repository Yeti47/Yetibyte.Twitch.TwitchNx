using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroTimeLine
{
    public class MacroTimeLineViewModel : ObservableObject
    {
        private readonly ObservableCollection<MacroTimeTrackViewModel> _tracks = new ObservableCollection<MacroTimeTrackViewModel>();
        private readonly Macro _macro;

        private float _scale = 1.0f;

        public IEnumerable<MacroTimeTrackViewModel> Tracks => _tracks;

        public TimeSpan EndTime => _tracks.Max(x => x.EndTime);

        public float Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                OnPropertyChanged();
            }
        }

        public MacroTimeLineViewModel(Macro macro)
        {
            _tracks.Add(new MacroTimeTrackViewModel());
            _tracks.Add(new MacroTimeTrackViewModel());
            _macro = macro;
        }
    }
}
