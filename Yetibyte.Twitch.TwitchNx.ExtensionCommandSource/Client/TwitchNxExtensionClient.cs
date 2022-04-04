using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers;
using RestSharp.Serializers.Json;
using Yetibyte.Twitch.TwitchNx.ExtensionCommandSource.Client.DataTransfer;

namespace Yetibyte.Twitch.TwitchNx.ExtensionCommandSource.Client
{
    public class TwitchNxExtensionClient
    {
        private const string MESSAGE_TYPE_SETUP_COMMANDS= "setup_commands";
        private const string MESSAGE_TYPE_RECEIVE_USER_COMMANDS = "receive_user_commands";

        private static readonly SetupCommandsResponse EMPTY_SETUP_COMMANDS_RESPONSE = new SetupCommandsResponse(0, 0);
        private static readonly ReceiveUserCommandsResponse EMPTY_RECEIVE_USER_COMMANDS_RESPONSE = new ReceiveUserCommandsResponse(Array.Empty<UserCommand>());

        private readonly RestClient _restClient;

        [DebuggerDisplay("****")]
        private readonly string _twitchNxClientId;

        public string BaseUrl { get; }

        public TwitchNxExtensionClient(string twitchNxClientId, string baseUrl)
        {
            _twitchNxClientId = twitchNxClientId;
            BaseUrl = baseUrl;

            _restClient = new RestClient(BaseUrl);
            _restClient.UseSystemTextJson(new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public SetupCommandsResponse SetupCommands(IEnumerable<Command> commands, IEnumerable<CooldownGroup> cooldownGroups)
        {
            CommandRequest commandRequest = new CommandRequest(
                _twitchNxClientId,
                MESSAGE_TYPE_SETUP_COMMANDS,
                commands.ToArray(),
                cooldownGroups.ToArray()
            );

            RestRequest restRequest = new RestRequest(string.Empty, Method.Post);
            restRequest.AddJsonBody(commandRequest);

            Task<SetupCommandsResponse?> setupCommandsTask = _restClient.PostAsync<SetupCommandsResponse?>(restRequest);

            if (setupCommandsTask is null)
                return EMPTY_SETUP_COMMANDS_RESPONSE;

            setupCommandsTask.Wait(); // Enforce synchronous behavior

            SetupCommandsResponse setupCommandsResponse = setupCommandsTask.Result ?? EMPTY_SETUP_COMMANDS_RESPONSE;

            return setupCommandsResponse;
        }

        public ReceiveUserCommandsResponse ReceiveCommands()
        {
            CommandRequest commandRequest = new CommandRequest(
                _twitchNxClientId,
                MESSAGE_TYPE_RECEIVE_USER_COMMANDS,
                Array.Empty<Command>(),
                Array.Empty<CooldownGroup>()
            );

            RestRequest restRequest = new RestRequest(string.Empty, Method.Post);
            restRequest.AddJsonBody(commandRequest);

            Task<ReceiveUserCommandsResponse?> receiveUserCommandsTask = _restClient.PostAsync<ReceiveUserCommandsResponse?>(restRequest);

            if (receiveUserCommandsTask is null)
                return EMPTY_RECEIVE_USER_COMMANDS_RESPONSE;

            receiveUserCommandsTask.Wait(); // Enforce synchronous behavior

            ReceiveUserCommandsResponse receiveUserCommandsResponse = receiveUserCommandsTask.Result ?? EMPTY_RECEIVE_USER_COMMANDS_RESPONSE;

            return receiveUserCommandsResponse;
        }

    }
}
