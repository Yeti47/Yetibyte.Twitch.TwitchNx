using GongSolutions.Wpf.DragDrop;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroTimeLine
{
    public class MacroTimeTrackViewModel : ObservableObject, IDropTarget
    {
        private const float LOOK_AHEAD_SECONDS = 3f;

        private readonly ObservableCollection<MacroTimeTrackElementViewModel> _elements = new ObservableCollection<MacroTimeTrackElementViewModel>();

        public IEnumerable<MacroTimeTrackElementViewModel> Elements => _elements;

        public TimeSpan EndTime
        {
            get
            {
                return (_elements.Any() ? _elements.Max(e => e.EndTime) : TimeSpan.Zero) + TimeSpan.FromSeconds(LOOK_AHEAD_SECONDS);   
            }
        }

        public MacroTimeTrackViewModel()
        {
            _elements.Add(new MacroTimeTrackElementViewModel { StartTime = TimeSpan.FromSeconds(1), Duration = TimeSpan.FromSeconds(2) });
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is MacroTimeTrackElementViewModel timeTrackElementVm)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is MacroTimeTrackElementViewModel timeTrackElementVm)
            {
                _elements.Add(timeTrackElementVm);
            }
        }
    }
}
