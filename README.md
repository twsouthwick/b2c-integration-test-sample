# Integration testing with Azure AD B2C and ASP.NET Core

Azure AD B2C is a great identity solution to get an application running quickly without needing to be concerned with many issues when maintaining a private identity store. However, integration testing applications that use AD B2C can be difficult as endpoints are often locked down or configured in some way to depend on a logged in user accessing them. This project aims to provide a framework that allows testing such endpoints by abstracting away the management of test users within a normal test flow. The design goals of this was to develop the framework in such a way that the test can be as declarative as possible without introducing boiler-plate code to manage the test users.

## Example
An example of a test using this framework is shown below that creates a user and then uses a user-defined service definition to make an HTTP call with the `HttpClient` provided by the test framework. The test then verifies that a call, which requires a user to make successfully, receives a response of `OK`.

```csharp
public class UserTest
{
    // An XUnit fixture that provides access to a hosted ASP.NET Core application and an HttpClient to access it
    private readonly ApplicationFixture _fixture;

    // A client-specific implementation of a service that takes an HttpClient and communicates with the hosted application
    private readonly ApplicationService _service;

    public UserTest(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _service = new ApplicationService(_fixture.Client);
    }

    [Fact]
    public async Task CallEndpointWithUser()
    {
        var user = await _fixture.CreateUserAsync();

        using(var result = await _service.GetValueUserAsync(user))
        {
            Assert.Equal(HttpStatusCode.Ok, result.StatusCode);
        }
    }
}
```

As can be seen, the act of retrieving a user is very easy and straightforward. This can be customized by providing an object that contains information about the state. For instance, in the sample project, an `admin` is someone whose username ends with `-admin` (best not to use such a system in production!). We need to configure the name generation to be dependent on this state. In order to do that, let's take a look at the definition of `ApplicationFixture`:

```csharp
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
```

This class derives from `AzureADB2CWebApplicationFactory<TStartup, TState>` which is provided by the test framework and builds upon the [ASP.NET Core integration test framework](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests). This class starts up an instance of the application under test, defined by the startup class supplied as the `TState` parameter. Various hooks are provided to customize this, including a way to configure the `IWebHostBuilder` itself, add custom services required for testing, and specify the AD B2C ROPC profile (this will be discussed later). In order to customize user configuration, we must supply an implementation of `UserConfigurator<TestUserState>` that appends *-admin* when the user is an admin:

```csharp
public class TestUserState
{
    public bool IsAdmin { get; set; }
}

public class AdminUserCreator : UserConfigurator<TestUserState>
{
    public override string CreateUsername(TestUserState state)
    {
        if (state?.IsAdmin == true)
        {
            return $"{base.CreateUsername(state)}-admin";
        }
        else
        {
            return base.CreateUsername(state);
        }
    }
}
```

## User creation

In order to create users, we use Azure Graph. There are plans to support doing so with the Microsoft Graph, but currently Azure Graph must be used for AD B2C users. The test fixture requires configuring the information for an AD application that has been granted the `User.ReadWrite.All` permission. This allows the test to add users and delete them after the testing is completed. In order to do this, create an application in Azure AD, then under `Required permissions->Windows Azure Active Directory` select 'Read and write directory data'. Once this is done, the information needs to be supplied into `AzureADB2CWebApplicationFactory<Startup, TestUserState>.Configure(GraphOptions options)`.

Once we have the ability to create users, we must then get access tokens for them. In order to do non-interactive logins with Azure AD B2C, we utilize the resource owner password credential ([ROPC](https://docs.microsoft.com/en-us/azure/active-directory-b2c/configure-ropc)) policy currently supplied as a public preview. This allows us to create a user with a given password and then retrieve an authorization token for testing. Since the tests are expected to be fairly quick, the testing framework at this time does not get refresh tokens but single use access tokens.