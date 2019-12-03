using System;
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
    public class of_which_sites_main_document_is_not_found : HydraClientTest
    {
        public override void ScenarioSetup()
        {
            base.ScenarioSetup();
            HttpCall.Setup(_ => _.HttpCall(It.IsAny<Uri>(), It.IsAny<IHttpOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Return.NotFound());
        }

        [Test]
        public void should_throw()
        {
            Client.Awaiting(_ => _.GetApiDocumentation(Resource.Of<IApiDocumentation>(BaseUrl).Object))
                .Should().Throw<InvalidResponseException>()
                .Which.Status.Should().Be(404);
        }
    }
}
