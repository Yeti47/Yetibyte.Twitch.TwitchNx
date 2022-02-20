using MahApps.Metro.Controls.Dialogs;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class NewProjectViewModel : ObservableObject
    {
        private readonly Action<NewProjectViewModel> _confirmHandler;
        private readonly Action<NewProjectViewModel> _cancelHandler;
        private readonly RelayCommand _confirmCommand;
        private readonly RelayCommand _cancelCommand;
        
        private string _projectName = string.Empty;

        public ICommand ConfirmCommand => _confirmCommand;
        public ICommand CancelCommand => _cancelCommand;

        public string ProjectName
        {
            get { return _projectName; }
            set { 
                _projectName = value;
                OnPropertyChanged();
                _confirmCommand.NotifyCanExecuteChanged();
            }
        }

        public NewProjectViewModel(Action<NewProjectViewModel> confirmHandler, Action<NewProjectViewModel> cancelHandler)
        {
            _confirmHandler = confirmHandler;
            _cancelHandler = cancelHandler;

            _cancelCommand = new RelayCommand(() => cancelHandler(this));
            _confirmCommand = new RelayCommand(() => confirmHandler(this), CanExecuteConfirmCommand);
        }

        private bool CanExecuteConfirmCommand()
        {
            return !string.IsNullOrWhiteSpace(_projectName);
        }

    }
}
