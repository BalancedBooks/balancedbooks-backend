using FluentAssertions;

namespace BalancedBooks_API.ExternalIntegrations.CompanyRegistry.Tests;

using NUnit.Framework;

[TestFixture]
public class CompanyRegistryHttpClientTests
{
    [SetUp]
    public void SetUp()
    {
        
    }

    [Test]
    public void Should()
    {
        var val = 10;

        val.Should().Be(10);
    }
}