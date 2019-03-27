using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Authentication.AzureAD.Testing
{
    /// <summary>
    /// Represents <see cref="User"/> information stored in Azure AD B2C.
    /// </summary>
    /// <remarks>
    /// For a complete list of properties:
    /// https://msdn.microsoft.com/en-us/library/azure/ad/graph/api/entity-and-complex-type-reference?f=255&MSPPError=-2147217396#user-entity
    /// </remarks>
    internal class AzureAdB2CUser
    {
        [JsonProperty("accountEnabled")]
        public bool? AccountEnabled { get; set; }

        [JsonProperty("objectId")]
        public string Id { get; set; }

        [JsonProperty("displayName")]
        public string Name { get; set; }

        [JsonProperty("creationType")]
        public string CreationType { get; set; }

        [JsonProperty("passwordProfile")]
        public PasswordProfile PasswordProfile { get; set; }

        [JsonProperty("passwordPolicies")]
        public string PasswordPolicies { get; set; }

        [JsonProperty("signInNames")]
        public KeyValueObject[] SignInNames => new[] { new KeyValueObject { Type = SignInTypes.EmailAddress, Value = Email } };

        [JsonIgnore]
        public string Email { get; set; }

        private class SignInTypes
        {
            public const string EmailAddress = "emailAddress";
            public const string Username = "userName";
        }
    }
}
