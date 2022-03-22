using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class MacroInstructionTemplateViewModel : ObservableObject
    {
        public record AnimationFrame(string ImagePath, float Duration);

        private readonly Func<IMacroInstruction> _instructionFactory;
        private readonly AnimationFrame[] _animationFrames;
        private readonly string _defaultImagePath;
        private bool _isAnimationPlaying;
        private string _imagePath = string.Empty;
        private string _toolTip = string.Empty;

        public string ToolTip
        {
            get { return _toolTip; }
            set { _toolTip = value; OnPropertyChanged(); }
        }

        public double AnimationDuration => _animationFrames.Sum(f => f.Duration);

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
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ImageUri));
            }
        }

        public string InitialImagePath => "/" + (!string.IsNullOrWhiteSpace(_defaultImagePath) ? _defaultImagePath : _animationFrames.First().ImagePath);

        public Uri ImageUri => new Uri(ImagePath, UriKind.Relative);

        public IEnumerable<AnimationFrame> AnimationFrames => _animationFrames;

        public MacroInstructionType MacroInstructionType { get; }

        public MacroInstructionTemplateViewModel(IEnumerable<AnimationFrame> animationFrames, MacroInstructionType macroInstructionType, Func<IMacroInstruction> instructionFactory, string defaultImagePath = "")
        {
            _animationFrames = animationFrames.ToArray();

            _defaultImagePath = defaultImagePath;

            if (!string.IsNullOrWhiteSpace(defaultImagePath))
                _imagePath = defaultImagePath;
            else if (_animationFrames.Any())
                _imagePath = _animationFrames.First().ImagePath;

            MacroInstructionType = macroInstructionType;
            _instructionFactory = instructionFactory;
        }

        public IMacroInstruction CreateMacroInstruction() => _instructionFactory();

        public MacroInstructionTemplateViewModel Clone()
        {
            MacroInstructionTemplateViewModel clone = new MacroInstructionTemplateViewModel(_animationFrames, MacroInstructionType, _instructionFactory, _defaultImagePath);

            return clone;
        }

    }
}
