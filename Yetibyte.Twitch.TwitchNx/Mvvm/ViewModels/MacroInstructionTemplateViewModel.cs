using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class MacroInstructionTemplateViewModel : ObservableObject
    {
        private readonly Func<IMacroInstruction> _instructionFactory;
        private readonly string _imageDirectory;
        private readonly double _keyFrameDurationSeconds;
        private bool _isAnimationPlaying;
        private string _imagePath = string.Empty;

        public double AnimationDuration => GetKeyFrameImagePaths().Count() * _keyFrameDurationSeconds;

        public double KeyFrameDuration => _keyFrameDurationSeconds;

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
            get => _imageDirectory;
            set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        //public Uri ImageUri
        //{
        //    get
        //    {
        //        Uri uri = new Uri("pack://application:,,,/Yetibyte.Twitch.TwitchNx;component/" + ImagePath.TrimStart('/'));
        //        return uri;
        //    }
        //}

        public MacroInstructionType MacroInstructionType { get; }

        public MacroInstructionTemplateViewModel(string imageDirectory, double keyFrameDurationSeconds, MacroInstructionType macroInstructionType, Func<IMacroInstruction> instructionFactory)
        {
            _imageDirectory = imageDirectory;
            _keyFrameDurationSeconds = keyFrameDurationSeconds;

            MacroInstructionType = macroInstructionType;
            _instructionFactory = instructionFactory;
        }

        public IMacroInstruction CreateMacroInstruction() => _instructionFactory();

        public MacroInstructionTemplateViewModel Clone()
        {
            MacroInstructionTemplateViewModel clone = new MacroInstructionTemplateViewModel(_imageDirectory, _keyFrameDurationSeconds, MacroInstructionType, _instructionFactory);

            return clone;
        }

        public IEnumerable<string> GetKeyFrameImagePaths()
        {
            List<string> keyFrameImagePaths = new List<string>();

            foreach (string pngFilePath in System.IO.Directory.EnumerateFiles(_imageDirectory, "*.gif"))
            {
                string relativePngFilePath = System.IO.Path.Combine(_imageDirectory, System.IO.Path.GetFileName(pngFilePath));

                keyFrameImagePaths.Add(relativePngFilePath);
            }

            return keyFrameImagePaths;
        }
    }
}
