using System;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.Testing;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.HydraClient_class.when_obtaining_an_API_documentation.from_a_valid_site
{
    [TestFixture]
    public class and_that_documentation_is_not_found : ScenarioTest
    {
        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            HttpCall.Setup(_ => _.HttpCall(new Uri(BaseUrl, "api/documentation"), It.IsAny<IHttpOptions>()))
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
