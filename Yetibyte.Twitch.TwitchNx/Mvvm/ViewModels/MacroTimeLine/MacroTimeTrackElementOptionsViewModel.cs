using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;
using Yetibyte.Twitch.TwitchNx.Styling;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroTimeLine
{
    [DefaultPane("MainBottom")]
    public class MacroTimeTrackElementOptionsViewModel : ToolViewModel
    {
        private object? _optionsViewModel;

        public object? OptionsViewModel
        {
            get => _optionsViewModel;
            private set
            {
                _optionsViewModel = value;
                OnPropertyChanged();
            }
        }

        public bool IsEmpty => _optionsViewModel is null;

        public MacroTimeTrackElementOptionsViewModel() : base("Macro Options")
        {
        }

        public void ShowOptions(object? optionsViewModel)
        {
            OptionsViewModel = optionsViewModel;
        }
    }
}
