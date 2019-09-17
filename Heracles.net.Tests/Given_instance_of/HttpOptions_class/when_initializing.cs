using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Heracles;
using NUnit.Framework;

namespace Given_instance_of.HttpOptions_class
{
    [TestFixture]
    public class when_initializing
    {
        [Test]
        public void Should_provide_correct_method()
        {
            new HttpOptions("POST").Method.Should().Be("POST");
        }

        [Test]
        public void Should_provide_correct_body()
        {
            new HttpOptions("GET", new MemoryStream()).Body.Should().BeOfType<MemoryStream>();
        }

        [Test]
        public void Should_provide_correct_headers()
        {
            new HttpOptions("GET", null, new Dictionary<string, string>() { { "Accept", "application/json" } })
                .Headers.Should().NotBeNull()
                .And.Subject.Should().ContainKey("Accept")
                .WhichValue.Should().Be("application/json");
        }

        [Test]
        public void Should_provide_no_headers_in_case_no_were_provided()
        {
            new HttpOptions().Headers.Should().NotBeNull().And.Subject.Should().BeEmpty();
        }
    }
}
