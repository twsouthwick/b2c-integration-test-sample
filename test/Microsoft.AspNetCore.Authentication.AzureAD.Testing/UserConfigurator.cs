using System;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authentication.AzureAD.Testing
{
    public class UserConfigurator<T>
    {
        private readonly string _testRunId = Guid.NewGuid().ToString();

        private int _userCount = 0;

        public virtual string CreateUsername(T state)
        {
            var sb = new StringBuilder();

            sb.Append("integration-test-");
            sb.Append(_testRunId);
            sb.Append('-');
            sb.Append(_userCount++);

            return sb.ToString();
        }

        public virtual Task ConfigureUserAsync(TestUser user, T state) => Task.CompletedTask;
    }
}
