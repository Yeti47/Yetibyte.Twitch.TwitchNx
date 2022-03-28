using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.ProjectManagement;
using FluentAssertions;

namespace Yetibyte.Twitch.TwitchNx.Test.Macros
{
    [TestClass]
    public class MacroTest
    {
        [TestMethod]
        public void SerializeMacro()
        {
            const int expectedTrackCount = 5;

            Macro macro = new Macro();
            
            for(int i = 0; i < expectedTrackCount; i++)
            {
                MacroTimeTrack timeTrack = macro.AddNewTimeTrack();

                timeTrack.Add(new MacroTimeTrackElement($"T{i}E0", TimeSpan.Zero, TimeSpan.FromSeconds(1.5), new ButtonPressMacroInstruction(ControllerButton.X)));
                timeTrack.Add(new MacroTimeTrackElement($"T{i}E1", TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(2), new ButtonPressMacroInstruction(ControllerButton.Y)));
                timeTrack.Add(new MacroTimeTrackElement($"T{i}E2", TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(4.25), new StickRotationMacroInstruction(ControllerStickDirection.North, ControllerStickDirection.South, ControllerStick.Right)));
                timeTrack.Add(new MacroTimeTrackElement($"T{i}E3", TimeSpan.FromSeconds(4.25), TimeSpan.FromSeconds(6), new StickRotationMacroInstruction(ControllerStickDirection.East, ControllerStickDirection.East, ControllerStick.Left)));
                timeTrack.Add(new MacroTimeTrackElement($"T{i}E4", TimeSpan.FromSeconds(7), TimeSpan.FromSeconds(8), new FixedStickDirectionMacroInstruction(new FixedStickDirectionInput { Stick = ControllerStick.Right, StickDirection = ControllerStickDirection.West })));
            
            }

            string macroJson = Newtonsoft.Json.JsonConvert.SerializeObject(macro, Newtonsoft.Json.Formatting.Indented);

            Macro? deserializedMacro = Newtonsoft.Json.JsonConvert.DeserializeObject<Macro>(macroJson);

            deserializedMacro.Should().BeEquivalentTo(macro);
        }
    }
}
