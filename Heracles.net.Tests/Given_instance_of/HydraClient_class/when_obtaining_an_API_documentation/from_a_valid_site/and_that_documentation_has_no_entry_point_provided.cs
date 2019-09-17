using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.Testing;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.HydraClient_class.when_obtaining_an_API_documentation.from_a_valid_site
{
    [TestFixture]
    public class and_that_documentation_has_no_entry_point_provided : ScenarioTest
    {
        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            var hypermedia = new Mock<IHypermediaContainer>(MockBehavior.Strict);
            hypermedia.Setup(_ => _.GetEnumerator()).Returns(new List<IResource>().GetEnumerator());
            HttpCall.Setup(_ => _.HttpCall(new Uri(BaseUrl, "api/documentation"), It.IsAny<IHttpOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Return.Ok());
            HypermediaProcessor
                .Setup(_ => _.Process(It.IsAny<IResponse>(), It.IsAny<IHydraClient>(), It.IsAny<IHypermediaProcessingOptions>()))
                .ReturnsAsync(hypermedia.Object);
        }

        [Test]
        public void should_throw()
        {
            Client.Awaiting(_ => _.GetApiDocumentation(Resource.Of<IApiDocumentation>(BaseUrl).Object))
                .Should().Throw<NoEntrypointDefinedException>();
        }
    }
}
