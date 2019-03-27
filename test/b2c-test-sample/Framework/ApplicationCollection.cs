using Xunit;

namespace b2c_test_sample.Framework
{
    [CollectionDefinition(nameof(ApplicationCollection))]
    public class ApplicationCollection : IClassFixture<ApplicationFixture>
    {
    }
}
