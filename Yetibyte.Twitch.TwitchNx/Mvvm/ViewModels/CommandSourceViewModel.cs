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
using Yetibyte.Twitch.TwitchNx.Core.Common;
using Yetibyte.Twitch.TwitchNx.Core.ProjectManagement;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;
using Yetibyte.Twitch.TwitchNx.Styling;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    [DefaultPane("MainRight")]
    public class CommandSourceViewModel : ToolViewModel, IDirtiable
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

        private const string TOOL_TITLE = "Command Source";
        private const string DIRTY_INDICATION = "*";

        private readonly ObservableCollection<CommandSourceItem> _commandSources = new ObservableCollection<CommandSourceItem>();
        private readonly IProjectManager _projectManager;
        private readonly ICommandSourceProvider _commandSourceProvider;
        private readonly RelayCommand _saveCommand;

        private CommandSourceItem _selectedCommandSource;
        private ICommandSourceSettingsViewModel _settingsViewModel;

        private bool _isDirty = false;

        public event EventHandler? IsDirtyChanged;

        public bool IsDirty
        {
            get => _isDirty;
            set { 
                _isDirty = value; 
                OnPropertyChanged();
                OnIsDirtyChanged();
            }
        }

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
                SettingsViewModel = SelectedCommandSource.Factory.CreateSettingsViewModel(this, _projectManager.CurrentProject);
                MarkDirty();
            }
        }


        public IEnumerable<CommandSourceItem> CommandSources => _commandSources;


        public ICommand SaveCommand => _saveCommand;


        public CommandSourceViewModel(IProjectManager projectManager, ICommandSourceProvider commandSourceProvider) :base(TOOL_TITLE)
        {
            _projectManager = projectManager;
            _commandSourceProvider = commandSourceProvider;

            PopulateCommandSources();

            _selectedCommandSource = _commandSources.First();
            _settingsViewModel = _commandSources.First().Factory.CreateSettingsViewModel(this, _projectManager.CurrentProject);

            _saveCommand = new RelayCommand(ExecuteSaveCommand);

            _projectManager.ProjectChanged += _projectManager_ProjectChanged;

            UnmarkDirty();
        }

        private void _projectManager_ProjectChanged(object? sender, EventArgs e)
        {
            if (_projectManager.CurrentProject is null) 
                return;

            var persistedCommandSource = _commandSources.FirstOrDefault(cs => cs.Factory.Id == _projectManager.CurrentProject?.CommandSourceFactory?.Id);

            if (persistedCommandSource != null)
            {
                SelectedCommandSource = persistedCommandSource;
                UnmarkDirty(); // We just loaded the project, so obviously the command source should not be marked as dirty.
            }
        }

        private void ExecuteSaveCommand()
        {
            if (_projectManager.CurrentProject is null)
                return;

            _projectManager.CurrentProject.CommandSourceFactory = SelectedCommandSource.Factory;

            var commandSourceSettings = _projectManager.CurrentProject.CommandSourceFactory.ApplySettings(SettingsViewModel);

            _projectManager.CurrentProject.WriteCommandSourceSettings(commandSourceSettings);

            UnmarkDirty();
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

        public void MarkDirty() => IsDirty = true;

        public void UnmarkDirty() => IsDirty = false;

        protected virtual void OnIsDirtyChanged()
        {
            var handler = IsDirtyChanged;
            handler?.Invoke(this, EventArgs.Empty);

            Title = TOOL_TITLE + (IsDirty ? DIRTY_INDICATION : string.Empty);
        }
    }
}
