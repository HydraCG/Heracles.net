using System;
using System.IO;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.Testing;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.HydraClient_class.when_obtaining_an_API_documentation.from_a_valid_site
{
    [TestFixture]
    public class and_that_documentation_is_provided_in_an_unsupported_format : ScenarioTest
    {
        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            HttpCall.Setup(_ => _.HttpCall(new Uri(BaseUrl, "api/documentation"), It.IsAny<IHttpOptions>()))
                .ReturnsAsync(Return.Ok(
                    new Uri(BaseUrl, "api/documentation"),
                    new MemoryStream(),
                    WithHeaders("Content-Type", "text/turtle")));
        }

        [Test]
        public void should_throw()
        {
            Client.Awaiting(_ => _.GetApiDocumentation(Resource.Of<IApiDocumentation>(BaseUrl).Object))
                .Should().Throw<ResponseFormatNotSupportedException>();
        }
    }
}
