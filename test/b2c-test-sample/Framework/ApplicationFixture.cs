using application;
using Microsoft.AspNetCore.Authentication.AzureAD.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace b2c_test_sample.Framework
{
    public class ApplicationFixture : AzureADB2CWebApplicationFactory<Startup, TestUserState>
    {
        private readonly IConfigurationRoot _configuration;

        public ApplicationFixture()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<ApplicationFixture>()
                .Build();
        }

        protected override string RopcProfile => "B2C_1_ROPC";

        protected override void Configure(GraphOptions options) => _configuration.Bind("MSGraph", options);

        protected override IWebHostBuilder Configure(IWebHostBuilder builder)
        {
            return builder.UseEnvironment("development");
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<UserConfigurator<TestUserState>, AdminUserCreator>();
        }
    }
}
