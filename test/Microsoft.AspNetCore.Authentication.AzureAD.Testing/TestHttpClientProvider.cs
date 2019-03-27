using System.Net.Http;

namespace Microsoft.AspNetCore.Authentication.AzureAD.Testing
{
    internal class TestHttpClientProvider
    {
        public TestHttpClientProvider(HttpClient client)
        {
            Client = client;
        }

        public HttpClient Client { get; }
    }
}
