using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.Testing;
using NUnit.Framework;

namespace Given_instance_of.HydraClient_class.when_obtaining_an_API_documentation
{
    [TestFixture]
    public class which_has_no_LINK_header_returned : HydraClientTest
    {
        [Test]
        public void should_throw()
        {
            Client.Awaiting(_ => _.GetApiDocumentation(Resource.Of<IApiDocumentation>(BaseUrl).Object))
                .Should().Throw<ApiDocumentationNotProvidedException>();
        }
    }
}
