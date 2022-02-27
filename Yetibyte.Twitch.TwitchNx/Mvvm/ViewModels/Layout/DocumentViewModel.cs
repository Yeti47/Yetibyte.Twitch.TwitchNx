using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Mvvm.Models;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout
{
    public abstract class DocumentViewModel : PaneViewModel
    {
        private readonly IDocumentManager _documentManager;
        private readonly RelayCommand _closeCommand;

        public abstract string DocumentName { get; }
        public abstract Type DocumentType { get; }

        public ICommand CloseCommand => _closeCommand;

        protected DocumentViewModel(IDocumentManager documentManager)
        {
            _documentManager = documentManager;

            _closeCommand = new RelayCommand(ExecuteCloseCommand, CanExecuteCloseCommand);
        }

        protected virtual bool CanExecuteCloseCommand()
        {
            return true;
        }

        protected virtual void ExecuteCloseCommand()
        {
            _documentManager.CloseDocument(DocumentType, DocumentName);
        }
    }
}
