using System;
using System.IO;
using System.Threading;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.Testing;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.HydraClient_class.when_obtaining_an_API_documentation
{
    [TestFixture]
    public class which_is_not_provided_within_the_LINK_header : HydraClientTest
    {
        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            HttpCall.Setup(_ => _.HttpCall(It.IsAny<Uri>(), It.IsAny<IHttpOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Return.Ok(
                    BaseUrl,
                    new MemoryStream(),
                    WithHeaders("Link", $"<{BaseUrl}api/documentation>; rel=\"next\"")));
        }

        [Test]
        public void should_throw()
        {
            Client.Awaiting(_ => _.GetApiDocumentation(Resource.Of<IApiDocumentation>(BaseUrl).Object))
                .Should().Throw<ApiDocumentationNotProvidedException>();
        }
    }
}
