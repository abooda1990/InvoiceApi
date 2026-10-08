using NUnit.Framework;

namespace InvoiceApi.Tests;

[TestFixture]
public class SmokeTests
{
    [Test]
    public void OnePlusOne_IsTwo()
    {
        var result = 1 + 1;

        Assert.That(result, Is.EqualTo(2));
    }
}
