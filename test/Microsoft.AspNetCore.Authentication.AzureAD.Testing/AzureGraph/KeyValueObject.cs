using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Authentication.AzureAD.Testing
{
    internal class KeyValueObject
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
