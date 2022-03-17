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

        public static IEnumerable<AnimationFrame> BuildSimpleButtonAnimation(string basePath, string button, float longDuration, float shortDuration)
        {
            for(int i = 1; i <= 4; i++)
            {
                yield return new AnimationFrame($"{basePath}btn_{button}/{button}_{i}.png", i <= 1 ? longDuration : shortDuration);
            }
        }

        public static IEnumerable<AnimationFrame> BuildFullCircleAnimation(string stickImagePath, string stick, float longDuration, float shortDuration)
        {
            for(int i = 0; i < 30; i++)
            {
                int frameIndex = i < 3 ? i : (3 + (i - 3) * 3);

                if (i >= 27)
                {
                    frameIndex = 3 - (i - 27);
                }

                float duration = i <= 0 ? longDuration : shortDuration;
                yield return new AnimationFrame($"{stickImagePath}{stick}/Joy_{stick}{frameIndex.ToString("00")}.png", duration);
            }
        }

    }
}
