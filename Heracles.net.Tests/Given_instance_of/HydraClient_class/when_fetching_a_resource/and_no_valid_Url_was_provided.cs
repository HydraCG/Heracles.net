using System;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.Testing;
using NUnit.Framework;

namespace Given_instance_of.HydraClient_class.when_fetching_a_resource
{
    [TestFixture]
    public class and_no_valid_Url_was_provided : ScenarioTest
    {
        [Test]
        public void should_throw()
        {
            Client.Awaiting(_ => _.GetResource(Resource.Of<IWebResource>(Resource.Null).Object))
                .Should().Throw<ArgumentNullException>()
                .Which.Message.Should().StartWith(HydraClient.NoUrlProvided);
        }
    }
}
