using System.Collections.Generic;
using System.Collections.ObjectModel;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroTimeLine
{
    public record StickDirectionViewModel(ControllerStickDirection ControllerStickDirection, string Name)
    {
        private static readonly List<StickDirectionViewModel> _controllerStickDirections = new List<StickDirectionViewModel>()
        {
            new StickDirectionViewModel(ControllerStickDirection.North, "North (0°)"),
            new StickDirectionViewModel(ControllerStickDirection.NorthEast, "North East (45°)"),
            new StickDirectionViewModel(ControllerStickDirection.East, "East (90°)"),
            new StickDirectionViewModel(ControllerStickDirection.SouthEast, "South East (135°)"),
            new StickDirectionViewModel(ControllerStickDirection.South, "South (180°)"),
            new StickDirectionViewModel(ControllerStickDirection.SouthWest, "South West (225°)"),
            new StickDirectionViewModel(ControllerStickDirection.West, "West (270°)"),
            new StickDirectionViewModel(ControllerStickDirection.NorthWest, "North West (315°)"),
        };

        public static IReadOnlyList<StickDirectionViewModel> All { get; } = new ReadOnlyCollection<StickDirectionViewModel>(_controllerStickDirections);
    }
}
