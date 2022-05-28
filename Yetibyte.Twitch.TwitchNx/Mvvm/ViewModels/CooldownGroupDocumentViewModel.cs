using System;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Mvvm.Models;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class CooldownGroupDocumentViewModel : DocumentViewModel
    {
        private readonly CooldownGroup _cooldownGroup;

        public override string DocumentName => _cooldownGroup.Name;

        public override Type DocumentType => _cooldownGroup.GetType();

        public CooldownGroupDocumentViewModel(CooldownGroup cooldownGroup, IDocumentManager documentManager) : base(documentManager)
        {
            _cooldownGroup = cooldownGroup;
            Title = DocumentName;
        }
    }
}
