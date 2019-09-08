using System;
using FluentAssertions;
using NUnit.Framework;

namespace Given_instance_of.HydraClient_class.when_invoking_an_operation
{
    [TestFixture]
    public class and_no_valid_operation_was_provided : ScenarioTest
    {
        [Test]
        public void should_throw()
        {
            Client.Awaiting(_ => _.Invoke(null))
                .Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("operation");
        }
    }
}
