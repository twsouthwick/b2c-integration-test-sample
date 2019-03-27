using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authentication.AzureAD.Testing
{
    internal class UserManager<T>
    {
        private readonly List<string> _users = new List<string>();

        private readonly HttpClient _httpClient;
        private readonly B2CGraphClient _graphClient;
        private readonly UserConfigurator<T> _userConfigurator;

        public UserManager(TestHttpClientProvider httpClient, B2CGraphClient graphClient, UserConfigurator<T> userConfigurator)
        {
            _httpClient = httpClient.Client;
            _graphClient = graphClient;
            _userConfigurator = userConfigurator;
        }

        public async Task<TestUser> CreateUserAsync(T state)
        {
            var username = _userConfigurator.CreateUsername(state);
            var password = Guid.NewGuid().ToString();

            var user = new AzureAdB2CUser
            {
                AccountEnabled = true,
                CreationType = "LocalAccount",
                Name = username,
                PasswordProfile = new PasswordProfile
                {
                    Password = password,
                    ForceChangePasswordNextLogin = false
                },
                PasswordPolicies = "DisablePasswordExpiration",
                Email = $"{username}@outlook.com"
            };

            var result = await _graphClient.CreateUserAsync(user);

            _users.Add(result.Id);

            var accessToken = await _graphClient.GetAccessTokenAsync(user.Email, password);
            var testUser = new TestUser(result.Id, username, password, accessToken);

            await _userConfigurator.ConfigureUserAsync(testUser, state);

            return testUser;
        }

        public Task RemoveAllTestUsersAsync()
        {
            return Task.CompletedTask;
        }
    }
}
