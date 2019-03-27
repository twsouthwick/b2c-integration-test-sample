using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authentication.AzureAD.Testing
{
    internal class WaitForUserCreationDelegatingHandler : DelegatingHandler
    {
        private const int MaxRetry = 10;
        private const string Error = "AADB2C99002";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            var count = 0;
            var delay = 50;

            while (count++ < MaxRetry)
            {
                response = await base.SendAsync(request, cancellationToken);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var message = await response.Content.ReadAsStringAsync();

                    if (message.Contains(Error, StringComparison.Ordinal))
                    {
                        response.Dispose();

                        await Task.Delay(delay);

                        delay *= 2;
                    }
                    else
                    {
                        throw new InvalidOperationException(message);
                    }
                }
                else
                {
                    return response;
                }
            }


            response.Dispose();

            throw new InvalidOperationException($"Max retries trying to access '{request.RequestUri}'");
        }
    }
}
