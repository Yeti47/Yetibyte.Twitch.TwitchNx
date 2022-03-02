using System;
using System.Collections.Generic;
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
        private const string MESSAGE_TYPE_GET_COMMANDS = "getcmds";

        private readonly RestClient _restClient;
        private readonly string _userUrl;

        public TwitchNxExtensionClient(string userUrl)
        {
            _restClient = new RestClient();
            _restClient.UseSystemTextJson(new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            _userUrl = userUrl;
        }

        public IEnumerable<Command> FetchCommands(string channel)
        {
            CommandRequest commandRequest = new CommandRequest
            {
                TwitchStreamer = channel,
                MessageType = MESSAGE_TYPE_GET_COMMANDS
            };

            RestRequest restRequest = new RestRequest(_userUrl, Method.Post);
            restRequest.AddJsonBody(commandRequest);

            Task<Command[]?>? fetchCommandsTask = _restClient.PostAsync<Command[]?>(restRequest);

            if (fetchCommandsTask is not null)
            {
                fetchCommandsTask.Wait();

                IEnumerable<Command>? commands = fetchCommandsTask.Result;

                return commands ?? Array.Empty<Command>();
            }
            else
            {
                return Array.Empty<Command>();  
            }
            
        }

    }
}
