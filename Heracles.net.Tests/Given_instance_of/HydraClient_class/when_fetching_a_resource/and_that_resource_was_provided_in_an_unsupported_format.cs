using System.IO;
using System.Threading;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.Testing;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.HydraClient_class.when_fetching_a_resource
{
    [TestFixture]
    public class and_that_resource_was_provided_in_an_unsupported_format : ScenarioTest
    {
        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            HttpCall.Setup(_ => _.HttpCall(ResourceUrl, It.IsAny<IHttpOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Return.Ok(
                    ResourceUrl,
                    new MemoryStream(),
                    WithHeaders("Content-Type", "text/turtle")));
        }

        [Test]
        public void should_throw()
        {
            Client.Awaiting(_ => _.GetResource(Resource.Of<IResource>(ResourceUrl).Object))
                .Should().Throw<ResponseFormatNotSupportedException>();
        }
    }
}
