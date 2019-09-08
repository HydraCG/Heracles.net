using System;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.Testing;
using NUnit.Framework;

namespace Given_instance_of.HydraClient_class.when_obtaining_an_API_documentation
{
    [TestFixture]
    public class and_no_valid_Url_is_given : HydraClientTest
    {
        [Test]
        public void should_throw()
        {
            Client.Awaiting(_ => _.GetApiDocumentation(Resource.Of<IApiDocumentation>(Resource.Null).Object))
                .Should().Throw<ArgumentNullException>()
                .Which.Message.Should().StartWith(HydraClient.NoUrlProvided);
        }
    }
}
