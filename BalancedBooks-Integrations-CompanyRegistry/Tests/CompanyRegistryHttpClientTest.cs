using FluentAssertions;
using NUnit.Framework;

namespace BalancedBooks_Integrations_CompanyRegistry.Tests;

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