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
        private const double DEFAULT_ELEMENT_DURATION = 0.5;
        public const long MIN_SNAP_STEP_TICKS = 100000;

        private readonly ObservableCollection<MacroTimeTrackElementViewModel> _elements = new ObservableCollection<MacroTimeTrackElementViewModel>();
        private readonly MacroTimeLineViewModel _timeLineViewModel;

        private long _snapStepTicks = 100000 * 5 / 2;

        public long SnapStepTicks
        {
            get => _snapStepTicks;
            set
            {
                _snapStepTicks = Math.Max(MIN_SNAP_STEP_TICKS, value);
            }
        }

        public IEnumerable<MacroTimeTrackElementViewModel> Elements => _elements;

        public TimeSpan EndTime
        {
            get
            {
                return (_elements.Any() ? _elements.Max(e => e.EndTime) : TimeSpan.Zero);  
            }
        }

        public double TrackWidth => _timeLineViewModel.TimeLineWidth;

        public float UnitsPerSecond => _timeLineViewModel.UnitsPerSecond;

        public MacroTimeTrackViewModel(MacroTimeLineViewModel timeLineViewModel)
        {
            _timeLineViewModel = timeLineViewModel;
            _timeLineViewModel.PropertyChanged += timeLineViewModel_PropertyChanged;
        }

        private void timeLineViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MacroTimeLineViewModel.TimeLineWidth))
            {
                OnPropertyChanged(nameof(TrackWidth));
            }
        }

        public TimeSpan SnapTimeSpan(TimeSpan input)
        {
            long inputTicks = input.Ticks;

            long remainderTicks = inputTicks % SnapStepTicks;

            long snappedTicks = inputTicks - remainderTicks;

            TimeSpan snappedTimeSpan = new TimeSpan(snappedTicks);

            return snappedTimeSpan;
        }

        public void NotifyUnitsPerSecondChanged()
        {
            OnPropertyChanged(nameof(UnitsPerSecond));

            foreach (var element in _elements)
                element.NotifyUnitsPerSecondChanged();
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is MacroTimeTrackElementViewModel timeTrackElementVm)
            {
                if (!timeTrackElementVm.IsDraggable)
                {
                    dropInfo.NotHandled = true;
                    return;
                }

                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = System.Windows.DragDropEffects.Move;

                if (!PlaceInTimeLine(dropInfo, timeTrackElementVm))
                {
                    //dropInfo.Effects = System.Windows.DragDropEffects.None;
                }
            }
            else if (dropInfo.Data is MacroToolBoxItemViewModel macroToolBoxItemViewModel)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = System.Windows.DragDropEffects.Copy;
            }
        }

        private bool PlaceInTimeLine(IDropInfo dropInfo, MacroTimeTrackElementViewModel timeTrackElementVm)
        {
            float startTimeSeconds = ((float)dropInfo.DropPosition.X - (float)timeTrackElementVm.HandlePosition.X) / UnitsPerSecond;

            if (startTimeSeconds < 0)
                startTimeSeconds = 0;

            TimeSpan originalStartTime = timeTrackElementVm.StartTime;

            TimeSpan targetStartTime = TimeSpan.FromSeconds(startTimeSeconds);

            timeTrackElementVm.StartTime = targetStartTime;

            MacroTimeTrackElementViewModel? firstOverlappingElement = null;

            do
            {
                int overlap = 0;

                firstOverlappingElement = _elements
                    .OrderBy(e => e.StartTime)
                    .FirstOrDefault(e => (overlap = e.Overlaps(timeTrackElementVm)) != 0);

                if (firstOverlappingElement is not null)
                {
                    timeTrackElementVm.StartTime = originalStartTime;
                    return false;
                    //targetStartTime = overlap < 0 
                    //    || (firstOverlappingElement.StartTime - timeTrackElementVm.Duration) < TimeSpan.Zero
                    //    ? firstOverlappingElement.EndTime 
                    //    : (firstOverlappingElement.StartTime - timeTrackElementVm.Duration);

                    //timeTrackElementVm.StartTime = targetStartTime;
                }
            }
            while (firstOverlappingElement != null);

            timeTrackElementVm.StartTime = targetStartTime;
            timeTrackElementVm.TimeTrack = this;

            OnPropertyChanged(nameof(EndTime));

            return true;

        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is MacroTimeTrackElementViewModel timeTrackElementVm && timeTrackElementVm.IsDraggable)
            {
                if (!PlaceInTimeLine(dropInfo, timeTrackElementVm))
                {
                }

                //OnPropertyChanged(nameof(TrackWidth));
            }
            else if (dropInfo.Data is MacroToolBoxItemViewModel macroToolBoxItemViewModel)
            {
                MacroTimeTrackElementViewModel macroTimeTrackElementViewModel = new MacroTimeTrackElementViewModel(
                    //macroToolBoxItemViewModel.MacroInstructionTemplateViewModel.Clone()
                    macroToolBoxItemViewModel.MacroInstructionTemplateViewModel,
                    Guid.NewGuid().ToString()
                );

                _timeLineViewModel.DeselectAllCommand.Execute(null);
                macroTimeTrackElementViewModel.IsSelected = true;

                macroTimeTrackElementViewModel.Duration = TimeSpan.FromSeconds(DEFAULT_ELEMENT_DURATION);

                PlaceInTimeLine(dropInfo, macroTimeTrackElementViewModel);
            }
        }

        public bool AddElement(MacroTimeTrackElementViewModel timeTrackElementVm)
        {
            if (!_elements.Contains(timeTrackElementVm))
            {
                _elements.Add(timeTrackElementVm);

                return true;
            };

            return false;
        }

        public bool RemoveElement(MacroTimeTrackElementViewModel timeTrackElementVm)
        {
            bool success = _elements.Remove(timeTrackElementVm);
            
            OnPropertyChanged(nameof(EndTime));

            return success;
        }

        public bool ContainsElement(MacroTimeTrackElementViewModel timeTrackElementVm)
        {
            return _elements.Contains(timeTrackElementVm);
        }

    }
}
