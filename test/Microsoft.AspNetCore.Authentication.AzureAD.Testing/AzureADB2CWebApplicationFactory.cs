using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authentication.AzureAD.Testing
{
    public abstract class AzureADB2CWebApplicationFactory<TStartup, TState> : IDisposable
        where TStartup : class
    {
        private static readonly Uri _baseAddress = new Uri("https://localhost");

        private readonly WebApplicationFactory<TStartup> _factory;
        private readonly Lazy<UserManager<TState>> _userManager;

        public AzureADB2CWebApplicationFactory()
        {
            (_factory, Client) = CreateFactory();
            _userManager = new Lazy<UserManager<TState>>(() => _factory.Server.Host.Services.GetRequiredService<UserManager<TState>>());
        }

        protected abstract string RopcProfile { get; }

        /// <summary>
        /// Configure a Azure AD Graph client that has the <c>Directory.ReadWrite.All</c> permission
        /// </summary>
        protected abstract void Configure(GraphOptions options);

        protected virtual IWebHostBuilder Configure(IWebHostBuilder builder) => builder;

        protected virtual void ConfigureServices(IServiceCollection services)
        {
        }

        private (WebApplicationFactory<TStartup>, HttpClient) CreateFactory()
        {
            IdentityModelEventSource.ShowPII = true;

            // We need to inject this client into some of the test services, but we don't create it till after the services
            // are registered. This allows us to lazily require it and have it available when we need it
            HttpClient client = null;

            var factory = new WebApplicationFactory<TStartup>()
                .WithWebHostBuilder(builder =>
                {
                    builder = Configure(builder);

                    if (builder is null)
                    {
                        throw new InvalidOperationException("Must return an IWebHostBuilder from derived Configure(IWebHostBuilder)");
                    }

                    builder.ConfigureServices(services =>
                    {
                        services.AddSingleton<WaitForUserCreationDelegatingHandler>();

                        services.AddHttpClient<B2CGraphClient>()
                            .AddHttpMessageHandler<WaitForUserCreationDelegatingHandler>();

                        services.AddOptions<GraphOptions>()
                            .Configure(Configure)
                            .ValidateDataAnnotations();

                        services.AddOptions<AzureADB2CProfiles>()
                            .Configure(options =>
                            {
                                options.ROPC = RopcProfile;
                            })
                            .ValidateDataAnnotations();


                        services.AddSingleton<UserManager<TState>>();
                        services.AddSingleton(ctx => new TestHttpClientProvider(client));
                        services.AddSingleton(typeof(UserConfigurator<>));

                        ConfigureServices(services);
                    });
                });

            // Set the test server client's base to HTTPS since the service requires it
            factory.ClientOptions.BaseAddress = _baseAddress;

            // Disable auto-redirects so that it's possible to validate the content of 3xx responses
            factory.ClientOptions.AllowAutoRedirect = false;

            client = factory.CreateDefaultClient(_baseAddress);

            return (factory, client);
        }

        public HttpClient Client { get; }

        public Task<TestUser> CreateUserAsync(TState state = default) => _userManager.Value.CreateUserAsync(state);

        public void Dispose() => _factory.Dispose();
    }
}
