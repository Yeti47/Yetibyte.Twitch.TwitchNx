using GongSolutions.Wpf.DragDrop;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class MacroToolBoxItemViewModel : ObservableObject, IDragSource
    {
        public MacroInstructionTemplateViewModel MacroInstructionTemplateViewModel { get; }


        public bool IsAnimationPlaying
        {
            get
            {
                return MacroInstructionTemplateViewModel.IsAnimationPlaying;
            }
            set
            {
                MacroInstructionTemplateViewModel.IsAnimationPlaying = value;
            }
        }

        public MacroToolBoxItemViewModel(MacroInstructionTemplateViewModel macroInstructionTemplateViewModel)
        {
            MacroInstructionTemplateViewModel = macroInstructionTemplateViewModel;
            MacroInstructionTemplateViewModel.PropertyChanged += MacroInstructionTemplateViewModel_PropertyChanged;
        }

        private void MacroInstructionTemplateViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MacroInstructionTemplateViewModel.IsAnimationPlaying))
            {
                OnPropertyChanged(nameof(IsAnimationPlaying));
            }
        }

        #region IDragSource Implementation

        void IDragSource.StartDrag(IDragInfo dragInfo)
        {
            dragInfo.Effects = DragDropEffects.Copy;
            dragInfo.Data = this;
        }

        bool IDragSource.CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        void IDragSource.Dropped(IDropInfo dropInfo)
        {
            dropInfo.VisualTarget.MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.Up));
        }

        void IDragSource.DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            
        }

        void IDragSource.DragCancelled()
        {
            
        }

        bool IDragSource.TryCatchOccurredException(Exception exception)
        {
            return false;
        }

        #endregion

    }
}
