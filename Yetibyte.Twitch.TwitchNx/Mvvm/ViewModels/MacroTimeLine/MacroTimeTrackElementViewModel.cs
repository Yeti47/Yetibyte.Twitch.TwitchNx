using GongSolutions.Wpf.DragDrop;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Windows;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroTimeLine
{
    public class MacroTimeTrackElementViewModel :ObservableObject, IDragSource
    {
        private TimeSpan _startTime;
        private TimeSpan _duration;

        public TimeSpan StartTime
        {
            get { return _startTime; }
            set { 
                _startTime = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(EndTime));
            }
        }

        public TimeSpan Duration
        {
            get { return _duration; }
            set { 
                _duration = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(EndTime));
            }
        }

        public TimeSpan EndTime => _startTime + _duration;

        #region IDragSource Implementation

        bool IDragSource.CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        void IDragSource.DragCancelled()
        {
            
        }

        void IDragSource.DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            
        }

        void IDragSource.Dropped(IDropInfo dropInfo)
        {
            
        }

        void IDragSource.StartDrag(IDragInfo dragInfo)
        {
            
        }

        bool IDragSource.TryCatchOccurredException(Exception exception)
        {
            return false;
        }

        #endregion
    }
}
