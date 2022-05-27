using log4net;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    public class TwitchIrcAuthTokenJsonConverter : JsonConverter
    {
        private static readonly byte[] ENTROPY_BYTES = new byte[] { 15, 70, 68, 12, 11, 129, 4 };

        public override bool CanConvert(Type objectType) => objectType == typeof(string);

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            string encryptedAuthToken = reader.Value?.ToString() ?? string.Empty;

            string authToken = string.Empty;

            try
            {
                byte[] encryptedAuthTokenData = Convert.FromBase64String(encryptedAuthToken);

                // TODO: Support decryption on other platforms
                byte[] decryptedAuthTokenData = ProtectedData.Unprotect(encryptedAuthTokenData, ENTROPY_BYTES, DataProtectionScope.LocalMachine);

                authToken = Encoding.UTF8.GetString(decryptedAuthTokenData);
            }
            catch (Exception ex)
            {
                ILog logger = LogManager.GetLogger("root");

                logger.Error("Auth token cannot be restored due to a decryption error.", ex);
            }

            return authToken;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            string authToken = value?.ToString() ?? string.Empty;

            byte[] authTokenData = Encoding.UTF8.GetBytes(authToken);

            string encryptedAuthToken = string.Empty;

            try
            {
                // TODO: Support encryption on other platforms
                byte[] encryptedAuthTokenData = ProtectedData.Protect(authTokenData, ENTROPY_BYTES, DataProtectionScope.LocalMachine);

                encryptedAuthToken = Convert.ToBase64String(encryptedAuthTokenData); 
            }
            catch (Exception ex)
            {
                ILog logger = LogManager.GetLogger("root");

                logger.Error("Auth token will not be saved due to an encryption error.", ex);

                writer.WriteNull();
            }

            writer.WriteValue(encryptedAuthToken);
        }
    }
}
