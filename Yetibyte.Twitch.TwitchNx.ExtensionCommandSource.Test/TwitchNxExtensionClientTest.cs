using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Yetibyte.Twitch.TwitchNx.ExtensionCommandSource.Client;
using Yetibyte.Twitch.TwitchNx.ExtensionCommandSource.Client.DataTransfer;

namespace Yetibyte.Twitch.TwitchNx.ExtensionCommandSource.Test
{
    [TestClass]
    public class TwitchNxExtensionClientTest
    {
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void SendCommandRequest()
        {
            string? channel = TestContext?.Properties["Channel"]?.ToString();
            string? userUrl = TestContext?.Properties["UserUrl"]?.ToString();

            if (string.IsNullOrWhiteSpace(channel))
            {
                Assert.Fail("Channel name not configured in test settings.");
                return;
            }

            if (string.IsNullOrWhiteSpace(userUrl))
            {
                Assert.Fail("User URL not configured in test settings.");
                return;
            }

            TwitchNxExtensionClient client = new TwitchNxExtensionClient(userUrl);

            IEnumerable<Command> commands = client.FetchCommands(channel);

            Assert.IsNotNull(commands);

        }
    }
}