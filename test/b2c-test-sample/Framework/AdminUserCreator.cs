using Microsoft.AspNetCore.Authentication.AzureAD.Testing;

namespace b2c_test_sample.Framework
{
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
}
