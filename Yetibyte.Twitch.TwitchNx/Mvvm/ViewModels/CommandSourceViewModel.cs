using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;
using Yetibyte.Twitch.TwitchNx.Core.ProjectManagement;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;
using Yetibyte.Twitch.TwitchNx.Styling;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    [DefaultPane("MainRight")]
    public class CommandSourceViewModel : ToolViewModel
    {
        public class CommandSourceItem : ObservableObject
        {
            private string _name;

            public ICommandSourceFactory Factory { get; }

            public string Name
            {
                get { return _name; }
                set { _name = value; OnPropertyChanged(); }
            }

            public CommandSourceItem(string name, ICommandSourceFactory factory)
            {
                _name = name;
                Factory = factory;
            }

        }

        private readonly ObservableCollection<CommandSourceItem> _commandSources = new ObservableCollection<CommandSourceItem>();
        private readonly IProjectManager _projectManager;
        private readonly ICommandSourceProvider _commandSourceProvider;
        private readonly RelayCommand _saveCommand;

        private CommandSourceItem _selectedCommandSource;
        private ICommandSourceSettingsViewModel _settingsViewModel;

        public ICommandSourceSettingsViewModel SettingsViewModel
        {
            get => _settingsViewModel;
            private set
            {
                _settingsViewModel = value;
                OnPropertyChanged();
            }
        }

        public CommandSourceItem SelectedCommandSource
        {
            get { return _selectedCommandSource; }
            set { 
                _selectedCommandSource = value; 
                OnPropertyChanged();
                SettingsViewModel = SelectedCommandSource.Factory.CreateSettingsViewModel();
            }
        }


        public IEnumerable<CommandSourceItem> CommandSources => _commandSources;


        public ICommand SaveCommand => _saveCommand;


        public CommandSourceViewModel(IProjectManager projectManager, ICommandSourceProvider commandSourceProvider) :base("Command Source")
        {
            _projectManager = projectManager;
            _commandSourceProvider = commandSourceProvider;

            PopulateCommandSources();

            _selectedCommandSource = _commandSources.First();
            _settingsViewModel = _commandSources.First().Factory.CreateSettingsViewModel();

            _saveCommand = new RelayCommand(ExecuteSaveCommand);
        }

        private void ExecuteSaveCommand()
        {
            if (_projectManager.CurrentProject is null)
                return;

            var commandSource = SelectedCommandSource.Factory.CreateCommandSource(_projectManager.CurrentProject.CommandSettings, SettingsViewModel);

            _projectManager.CommandSource = commandSource;
        }

        private void PopulateCommandSources()
        {
            _commandSources.Clear();

            foreach(var commandSourceFactory in _commandSourceProvider.GetCommandSourceFactories())
            {
                CommandSourceItem commandSourceVm = new CommandSourceItem(commandSourceFactory.SourceDisplayName, commandSourceFactory);

                _commandSources.Add(commandSourceVm);
            }
        }
    }
}
