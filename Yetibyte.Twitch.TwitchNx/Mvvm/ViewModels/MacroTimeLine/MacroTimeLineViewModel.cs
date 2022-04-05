using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;
using Yetibyte.Twitch.TwitchNx.Services;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroTimeLine
{
    public class MacroTimeLineViewModel : ObservableObject
    {
        public class AxisLabel
        {
            public TimeSpan TimeStamp { get; set; }
            public string Text => TimeStamp.ToString(@"mm\:ss\.fff");
            public double Width { get; set; }
        }

        public enum ZoomLevels
        {
            Percent10 = 10,
            Percent25 = 25,
            Percent50 = 50,
            Percent75 = 75,
            Percent100 = 100,
            Perecent125 = 125,
            Percent150 = 150,
            Percent200 = 200,
            Percent250 = 250,
            Percent300 = 300,
            Percent500 = 500
        }

        public record ZoomStep(ZoomLevels ZoomLevel, string Text);

        public const int DEFAULT_UNITS_PER_SECOND = 100;
        public const int MIN_LABEL_DISTANCE = 50;
        public const int TRACK_COUNT = 5;
        private const int DEFAULT_DURATION_SECONDS = 60;
        private const ZoomLevels MIN_ZOOM_LEVEL = ZoomLevels.Percent50;

        private readonly ZoomStep[] _zoomSteps;

        private readonly ObservableCollection<MacroTimeTrackViewModel> _tracks = new ObservableCollection<MacroTimeTrackViewModel>();
        private readonly ObservableCollection<AxisLabel> _axisLabels = new ObservableCollection<AxisLabel>();

        private readonly RelayCommand _zoomInCommand;
        private readonly RelayCommand _zoomOutCommand;
        private readonly RelayCommand _deselectAllCommand;
        private readonly RelayCommand _deleteSelectedElementsCommand;
        private readonly RelayCommand _exportToClipboardCommand;
        private readonly RelayCommand _testMacroCommand;

        private readonly IMacroInstructionTemplateFactoryFacade _macroInstructionTemplateFactoryFacade;
        private readonly Macro _macro;
        private readonly ISwitchControllerSelector _switchControllerSelector;
        private readonly SwitchConnector _switchConnector;
        private ZoomLevels _zoomlevel = ZoomLevels.Percent100;

        private TimeSpan _targetDuration;
        private float _unitsPerSecond = DEFAULT_UNITS_PER_SECOND;
        private ZoomStep _selectedZoomStep;

        public ZoomStep SelectedZoomStep
        {
            get { return _selectedZoomStep; }
            set { 
                _selectedZoomStep = value;
                OnPropertyChanged();

                ZoomLevel = _selectedZoomStep.ZoomLevel;
            }
        }

        public float UnitsPerSecond
        {
            get { return _unitsPerSecond * Scale; }
            set { 
                _unitsPerSecond = value;

                RegenerateAxis();

                NotifyUnitsPerSecondChanged();
                NotifyTimeLineWidthChanged();
            }
        }

        public ZoomLevels ZoomLevel
        {
            get => _zoomlevel;
            private set
            {
                _zoomlevel = value;

                if(_selectedZoomStep.ZoomLevel != _zoomlevel)
                    _selectedZoomStep = _zoomSteps.First(zs => zs.ZoomLevel == _zoomlevel);

                RegenerateAxis();

                NotifyUnitsPerSecondChanged();
                NotifyTimeLineWidthChanged();
                OnPropertyChanged(nameof(Scale));

                _zoomInCommand.NotifyCanExecuteChanged();
                _zoomOutCommand.NotifyCanExecuteChanged();
            }
        }

        public TimeSpan TargetDuration
        {
            get { return _targetDuration; }
            set
            {
                _targetDuration = value;

                RegenerateAxis();

                OnPropertyChanged();
                NotifyTimeLineWidthChanged();
            }
        }

        public double TimeLineWidth => _targetDuration.TotalSeconds * UnitsPerSecond;

        public IEnumerable<MacroTimeTrackViewModel> Tracks => _tracks;

        public IEnumerable<AxisLabel> AxisLabels => _axisLabels;

        public TimeSpan EndTime => _tracks.Max(x => x.EndTime);

        public float Scale => ((int)_zoomlevel) / 100f;

        public IEnumerable<ZoomStep> ZoomSteps => _zoomSteps;

        public ICommand ZoomInCommand => _zoomInCommand;
        public ICommand ZoomOutCommand => _zoomOutCommand;
        public ICommand DeselectAllCommand => _deselectAllCommand;
        public ICommand DeleteSelectedElementsCommand => _deleteSelectedElementsCommand;
        public ICommand ExportToClipboardCommand => _exportToClipboardCommand;
        public ICommand TestMacroCommand => _testMacroCommand;

        public MacroTimeLineViewModel(IMacroInstructionTemplateFactoryFacade macroInstructionTemplateFactoryFacade, Macro macro, ISwitchControllerSelector switchControllerSelector, SwitchConnector switchConnector)
        {
            _macroInstructionTemplateFactoryFacade = macroInstructionTemplateFactoryFacade;
            _macro = macro;
            _switchControllerSelector = switchControllerSelector;
            _switchConnector = switchConnector;
            _zoomlevel = ZoomLevels.Percent100;

            _zoomSteps = Enum.GetValues<ZoomLevels>()
                .Where(zl => zl >= MIN_ZOOM_LEVEL)
                .Select(zl => new ZoomStep(zl, $"{(int)zl} %")).ToArray();

            _selectedZoomStep = _zoomSteps.First(zs => zs.ZoomLevel == _zoomlevel);

            _zoomInCommand = new RelayCommand(ExecuteZoomInCommand, CanExecuteZoomInCommand);
            _zoomOutCommand = new RelayCommand(ExecuteZoomOutCommand, CanExecuteZoomOutCommand);

            _deselectAllCommand = new RelayCommand(ExecuteDeselectAllCommand);
            _deleteSelectedElementsCommand = new RelayCommand(ExecuteDeleteSelectedElementsCommand);

            TargetDuration = TimeSpan.FromSeconds(DEFAULT_DURATION_SECONDS);

            InitializeTimeLine();

            _exportToClipboardCommand = new RelayCommand(ExecuteExportToClipboardCommand, CanExecuteExportToClipboardCommand);
            _testMacroCommand = new RelayCommand(ExecuteTestMacroCommand, CanExecuteTestMacroCommand);

            _tracks.CollectionChanged += tracks_CollectionChanged;
        }

        private void tracks_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems is not null)
            {
                foreach(MacroTimeTrackViewModel track in e.OldItems)
                    track.ElementCollectionChanged -= Track_ElementCollectionChanged;
            }

            if (e.NewItems is not null)
            {
                foreach (MacroTimeTrackViewModel track in e.NewItems)
                    track.ElementCollectionChanged += Track_ElementCollectionChanged;
            }

        }

        private void Track_ElementCollectionChanged(object? sender, EventArgs e)
        {
            _testMacroCommand.NotifyCanExecuteChanged();
        }

        private void NotifyTimeLineWidthChanged()
        {
            OnPropertyChanged(nameof(TimeLineWidth));

            foreach (var track in _tracks)
                track.NotifyTrackWidthChanged();
        }

        private bool CanExecuteTestMacroCommand()
        {
            return _tracks.Any(t => t.Elements.Any());
        }

        private void ExecuteTestMacroCommand()
        {
            if (_switchControllerSelector.SelectedController is null)
            {
                // TODO: Display error message either by using an injected service or by raising an event.
                return;
            }

            if (_switchConnector.State != SwitchConnectorState.Connected || !_switchConnector.IsConnectedToSwitch)
            {
                // TODO: Display error message either by using an injected service or by raising an event.
                return;
            }

            string macroText = BuildMacroString();

            _switchConnector.ExecuteMacro(macroText, _switchControllerSelector.SelectedController.Id);
        }

        private void InitializeTimeLine()
        {
            for (int i = 0; i < TRACK_COUNT; i++)
            {
                if (_macro.TimeTrackCount > i)
                {
                    AddTrackFromModel(_macro.TimeTracks.ElementAt(i));
                }
                else
                {
                    MacroTimeTrackViewModel trackVm = new MacroTimeTrackViewModel(this);
                    _tracks.Add(trackVm);
                }

            }
        }

        private MacroTimeTrackViewModel AddTrackFromModel(MacroTimeTrack macroTimeTrack)
        {
            MacroTimeTrackViewModel timeTrackViewModel = new MacroTimeTrackViewModel(this);

            _tracks.Add(timeTrackViewModel);

            foreach (var trackElement in macroTimeTrack)
            {
                MacroInstructionTemplateViewModel macroInstructionTemplateViewModel = _macroInstructionTemplateFactoryFacade.CreateFor(trackElement.Instruction);

                MacroTimeTrackElementViewModel trackElementViewModel = new MacroTimeTrackElementViewModel(macroInstructionTemplateViewModel, trackElement.Id);
                
                timeTrackViewModel.AddElement(trackElementViewModel);

                trackElementViewModel.StartTime = trackElement.StartTime;
                trackElementViewModel.Duration = trackElement.Duration;
            }

            return timeTrackViewModel;
        }

        private bool CanExecuteExportToClipboardCommand()
        {
            return true;
        }

        private void ExecuteExportToClipboardCommand()
        {
            ExportToClipboard();
        }

        private void ExecuteDeleteSelectedElementsCommand()
        {
            var selectedElements = _tracks.SelectMany(t => t.Elements).Where(e => e.IsSelected).ToArray();

            foreach (var element in selectedElements)
            {
                element.DeleteCommand.Execute(null);
            }
        }

        private void ExecuteDeselectAllCommand()
        {
            foreach (var element in _tracks.SelectMany(t => t.Elements))
                element.IsSelected = false;
                
        }

        private bool CanExecuteZoomOutCommand() => _zoomlevel > ZoomLevels.Percent10;

        private void ExecuteZoomOutCommand()
        {
            _zoomlevel = Enum.GetValues<ZoomLevels>()
                .OrderByDescending(zl => (int)zl)
                .FirstOrDefault(zl => zl < _zoomlevel);
        }

        private bool CanExecuteZoomInCommand() => _zoomlevel < ZoomLevels.Percent500;

        private void ExecuteZoomInCommand()
        {
            _zoomlevel = Enum.GetValues<ZoomLevels>()
                .OrderBy(zl => (int)zl)
                .FirstOrDefault(zl => zl > _zoomlevel);
        }

        private bool RegenerateAxis()
        {
            _axisLabels.Clear();

            foreach (var axisLabel in BuildAxisLabels())
            {
                _axisLabels.Add(axisLabel);
            }

            OnPropertyChanged(nameof(AxisLabels));

            return true;
        }

        private IEnumerable<AxisLabel> BuildAxisLabels()
        {
            float secondScale = 1;

            int labelCount = (int)Math.Floor(TimeLineWidth / UnitsPerSecond) - 1;

            while ((UnitsPerSecond * secondScale) / 2 >= MIN_LABEL_DISTANCE)
            {
                secondScale /= 2;
                labelCount *= 2;
            }

            for (int i = 0; i <= labelCount; i++)
            {
                AxisLabel axisLabel = new AxisLabel
                {
                    TimeStamp = TimeSpan.FromSeconds((i+ 1) * secondScale),
                    Width = UnitsPerSecond * secondScale
                };

                yield return axisLabel;
            }
        }

        private void NotifyUnitsPerSecondChanged()
        {
            OnPropertyChanged(nameof(UnitsPerSecond));

            foreach (var track in _tracks)
                track.NotifyUnitsPerSecondChanged();
        }

        private void PopulateMacro(Macro macro)
        {
            macro.Clear();

            foreach (var timeTackVm in _tracks)
            {
                MacroTimeTrack macroTimeTrack = macro.AddNewTimeTrack();

                foreach (var timeTrackElementVm in timeTackVm.Elements)
                {
                    IMacroInstruction macroInstruction = timeTrackElementVm.InstructionTemplateViewModel.CreateMacroInstruction();

                    MacroTimeTrackElement macroTimeTrackElement = new MacroTimeTrackElement(
                        timeTrackElementVm.Id,
                        timeTrackElementVm.StartTime,
                        timeTrackElementVm.EndTime,
                        macroInstruction
                    );

                    macroTimeTrack.Add(macroTimeTrackElement);
                }
            }
        }

        public void ApplyChanges()
        {
            PopulateMacro(_macro);
        }

        public string BuildMacroString()
        {
            Macro macro = new Macro();

            PopulateMacro(macro);

            string macroString = macro.Build();

            return macroString;
        }

        public string ExportToClipboard()
        {
            string macroString = BuildMacroString();

            System.Windows.Clipboard.SetText(macroString);

            return macroString;
        }

    }
}
