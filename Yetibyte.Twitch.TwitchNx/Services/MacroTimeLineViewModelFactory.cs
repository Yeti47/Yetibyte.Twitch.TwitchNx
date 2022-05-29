using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroTimeLine;
using Yetibyte.Twitch.TwitchNx.Services.Dialog;

namespace Yetibyte.Twitch.TwitchNx.Services
{
    public class MacroTimeLineViewModelFactory : IMacroTimeLineViewModelFactory
    {
        private readonly IMacroInstructionTemplateFactoryFacade _macroInstructionTemplateFactoryFacade;
        private readonly SwitchConnector _switchConnector;
        private readonly ISwitchControllerSelector _switchControllerSelector;
        private readonly IDialogService _dialogService;

        public MacroTimeLineViewModelFactory(IMacroInstructionTemplateFactoryFacade macroInstructionTemplateFactoryFacade, SwitchConnector switchConnector, ISwitchControllerSelector switchControllerSelector, IDialogService dialogService)
        {
            _macroInstructionTemplateFactoryFacade = macroInstructionTemplateFactoryFacade;
            _switchConnector = switchConnector;
            _switchControllerSelector = switchControllerSelector;
            _dialogService = dialogService;
        }

        public MacroTimeLineViewModel CreateViewModel(Macro macro)
        {
            return new MacroTimeLineViewModel(_macroInstructionTemplateFactoryFacade, macro, _switchControllerSelector, _switchConnector, _dialogService);
        }
    }
}
