using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class MacroInstructionTemplateViewModel : ObservableObject
    {
        private readonly Func<IMacroInstruction> _instructionFactory;
        private string _imagePath;
        private bool _isAnimationPlaying;


        public bool IsAnimationPlaying
        {
            get { return _isAnimationPlaying; }
            set
            {
                _isAnimationPlaying = value;
                OnPropertyChanged();
            }
        }


        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; OnPropertyChanged(); }
        }

        public Uri ImageUri
        {
            get
            {
                Uri uri = new Uri("pack://application:,,,/Yetibyte.Twitch.TwitchNx;component/" + ImagePath.TrimStart('/'));
                return uri;
            }
        }

        public MacroInstructionType MacroInstructionType { get; }

        public MacroInstructionTemplateViewModel(string imagePath, MacroInstructionType macroInstructionType, Func<IMacroInstruction> instructionFactory)
        {
            _imagePath = imagePath;
            MacroInstructionType = macroInstructionType;
            _instructionFactory = instructionFactory;
        }

        public IMacroInstruction CreateMacroInstruction() => _instructionFactory();

        public MacroInstructionTemplateViewModel Clone()
        {
            MacroInstructionTemplateViewModel clone = new MacroInstructionTemplateViewModel(ImagePath, MacroInstructionType, _instructionFactory);

            return clone;
        }
    }
}
