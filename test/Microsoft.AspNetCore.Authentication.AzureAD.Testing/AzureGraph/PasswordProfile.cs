using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Authentication.AzureAD.Testing
{
    internal class PasswordProfile
    {
        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("forceChangePasswordNextLogin", DefaultValueHandling = DefaultValueHandling.Include)]
        public bool ForceChangePasswordNextLogin { get; set; }
    }
}
