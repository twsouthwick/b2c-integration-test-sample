namespace Microsoft.AspNetCore.Authentication.AzureAD.Testing
{
    public class TestUser
    {
        public TestUser(string id, string username, string password, string accessToken)
        {
            Id = id;
            Username = username;
            Password = password;
            AccessToken = accessToken;
        }

        public string Id { get; }

        public string Password { get; }

        public string Username { get; }

        public string AccessToken { get; }
    }
}
