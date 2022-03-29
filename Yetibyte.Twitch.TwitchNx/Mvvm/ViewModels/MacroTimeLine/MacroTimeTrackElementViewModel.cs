using GongSolutions.Wpf.DragDrop;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroTimeLine
{
    public class MacroTimeTrackElementViewModel : ObservableObject, IDragSource
    {
        public const float MINIMUM_DURATION_SECONDS = 0.1f;

        private readonly RelayCommand _deleteCommand;
        private readonly RelayCommand _selectCommand;

        private TimeSpan _startTime;
        private TimeSpan _duration;
        private MacroTimeTrackViewModel? _timeTrack;

        private MacroTimeTrackViewModel? _originalTimeTrack;
        private TimeSpan? _originalStartTime;
        private MacroInstructionTemplateViewModel _instructionTemplateViewModel;
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { 
                _isSelected = value; 
                OnPropertyChanged(); 
            }
        }

        public ICommand DeleteCommand => _deleteCommand;

        public ICommand SelectCommand => _selectCommand;

        public Point HandlePosition { get; private set; }

        public MacroInstructionTemplateViewModel InstructionTemplateViewModel
        {
            get { return _instructionTemplateViewModel; }
            set { _instructionTemplateViewModel = value; OnPropertyChanged(); }
        }

        public TimeSpan StartTime
        {
            get { return _startTime; }
            set { 
                _startTime = value;

                if (_timeTrack is not null)
                    _startTime = _timeTrack.SnapTimeSpan(_startTime);

                OnPropertyChanged();
                OnPropertyChanged(nameof(EndTime));
            }
        }

        public TimeSpan Duration
        {
            get { return _duration; }
            set { 
                _duration = value;

                if (_timeTrack is not null)
                    _duration = _timeTrack.SnapTimeSpan(_duration);

                OnPropertyChanged();
                OnPropertyChanged(nameof(EndTime));

            }
        }

        public TimeSpan EndTime
        {
            get
            {
                TimeSpan endTime = _startTime + _duration;

                return _timeTrack is not null ? _timeTrack.SnapTimeSpan(endTime) : endTime;
            }
        }

        public bool IsDraggable { get; set; } = true;

        public float UnitsPerSecond => _timeTrack?.UnitsPerSecond ?? MacroTimeLineViewModel.DEFAULT_UNITS_PER_SECOND;


        public MacroTimeTrackViewModel? TimeTrack
        {
            get => _timeTrack;
            set
            {
                if (_timeTrack != value)
                {
                    _timeTrack?.RemoveElement(this);

                    _timeTrack = value;

                    if (!_timeTrack?.ContainsElement(this) ?? false)
                    {
                        _timeTrack?.AddElement(this);
                    }

                    OnPropertyChanged();
                }
            }
        }

        public string Id { get; }

        public MacroTimeTrackElementViewModel(MacroInstructionTemplateViewModel instructionTemplateViewModel, string id)
        {
            _instructionTemplateViewModel = instructionTemplateViewModel;
            Id = id;

            _deleteCommand = new RelayCommand(() => this.TimeTrack = null);
            _selectCommand = new RelayCommand(() => this.IsSelected = true);
        }

        public void AdjustDurationByUnits(double units)
        {
            double seconds = units / UnitsPerSecond;

            TimeSpan targetDuration = Duration + TimeSpan.FromSeconds(seconds);

            if (_timeTrack is not null)
                targetDuration = _timeTrack.SnapTimeSpan(targetDuration);

            if (_timeTrack?.Elements
                .OrderBy(e => e.StartTime)
                .FirstOrDefault(e => e != this 
                    && e.StartTime > StartTime && e.StartTime < (StartTime + targetDuration)) is MacroTimeTrackElementViewModel firstCollision)
            {
                targetDuration = firstCollision.StartTime - StartTime;

                if (_timeTrack is not null)
                    targetDuration = _timeTrack.SnapTimeSpan(targetDuration);
            }

            if (targetDuration <= TimeSpan.FromSeconds(MINIMUM_DURATION_SECONDS))
                targetDuration = TimeSpan.FromSeconds(MINIMUM_DURATION_SECONDS);

            Duration = targetDuration;
        }

        public void AdjustStartTimeByUnits(double units)
        {
            double seconds = units / UnitsPerSecond;

            TimeSpan targetStartTime = StartTime + TimeSpan.FromSeconds(seconds);

            if (_timeTrack is not null)
                targetStartTime = _timeTrack.SnapTimeSpan(targetStartTime);

            if (EndTime - targetStartTime < TimeSpan.FromSeconds(MINIMUM_DURATION_SECONDS))
                targetStartTime = EndTime - TimeSpan.FromSeconds(MINIMUM_DURATION_SECONDS);

            if (_timeTrack is not null)
                targetStartTime = _timeTrack.SnapTimeSpan(targetStartTime);

            if (targetStartTime < TimeSpan.Zero)
                targetStartTime = TimeSpan.Zero;

            if (_timeTrack?.Elements
                .OrderBy(e => e.EndTime)
                .FirstOrDefault(e => e != this 
                    && e.EndTime > targetStartTime
                    && e.StartTime < StartTime) is MacroTimeTrackElementViewModel firstCollision)
            {
                targetStartTime = firstCollision.EndTime;

                if (_timeTrack is not null)
                    targetStartTime = _timeTrack.SnapTimeSpan(targetStartTime);
            }

            TimeSpan oldStartTime = StartTime;

            StartTime = targetStartTime;

            Duration = Duration + (oldStartTime - StartTime);

            if (Duration < TimeSpan.FromSeconds(MINIMUM_DURATION_SECONDS))
                Duration = TimeSpan.FromSeconds(MINIMUM_DURATION_SECONDS);

        }

        public int Overlaps(MacroTimeTrackElementViewModel other)
        {
            if (other == this)
                return 0;

            if (this.StartTime <= other.StartTime && this.EndTime > other.StartTime)
                return -1;
            if (this.StartTime < other.EndTime && this.EndTime >= other.EndTime)
                return 1;

            if ((this.StartTime < other.EndTime && this.StartTime > other.StartTime)
                || (this.EndTime < other.EndTime && this.EndTime >= other.EndTime)) {

                if (!(this.EndTime < other.EndTime && this.EndTime >= other.EndTime))
                    return 1;

                if (!(this.StartTime < other.EndTime && this.StartTime > other.StartTime))
                    return -1;


                return Math.Sign(other.StartTime.TotalSeconds - this.StartTime.TotalSeconds);

            }

            return 0;
        }

        public void NotifyUnitsPerSecondChanged()
        {
            OnPropertyChanged(nameof(UnitsPerSecond));
        }

        #region IDragSource Implementation

        bool IDragSource.CanStartDrag(IDragInfo dragInfo)
        {
            return IsDraggable;
        }

        void IDragSource.DragCancelled()
        {
            if (_originalTimeTrack is not null)
                TimeTrack = _originalTimeTrack;

            if (_originalStartTime is not null)
                StartTime = _originalStartTime.Value;
        }

        void IDragSource.DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            System.GC.Collect();
        }

        void IDragSource.Dropped(IDropInfo dropInfo)
        {
            
        }

        void IDragSource.StartDrag(IDragInfo dragInfo)
        {
            HandlePosition = dragInfo.PositionInDraggedItem;

            _originalTimeTrack = _timeTrack;
            _originalStartTime = _startTime;

            dragInfo.Effects = IsDraggable ? DragDropEffects.Move : DragDropEffects.None;

            dragInfo.Data = this;
            
        }

        bool IDragSource.TryCatchOccurredException(Exception exception)
        {
            return false;
        }

        #endregion
    }
}
