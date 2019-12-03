using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Heracles;
using Heracles.DataModel;
using Heracles.Namespaces;
using Heracles.Testing;
using Moq;
using NUnit.Framework;
using RDeF.Entities;

namespace Given_instance_of.HydraClient_class.when_obtaining_an_API_documentation.from_a_valid_site
{
    [TestFixture]
    public class which_is_provided_correctly : ScenarioTest
    {
        private IResponse Response { get; set; }

        private Uri ApiDocumentationUrl { get; set; }

        public override void ScenarioSetup()
        {
            base.ScenarioSetup();
            ApiDocumentationUrl = new Uri(BaseUrl, "api/documentation");
            var apiDocumentation = new Mock<IApiDocumentation>(MockBehavior.Strict);
            apiDocumentation.SetupGet(_ => _.EntryPoint).Returns(Resource.Of<IResource>(new Uri(BaseUrl, "api")).Object);
            apiDocumentation.SetupGet(_ => _.Type).Returns(new HashSet<Iri>() { hydra.ApiDocumentation });
            var hypermedia = new Mock<IHypermediaContainer>(MockBehavior.Strict);
            hypermedia.Setup(_ => _.GetEnumerator())
                .Returns(new List<IResource>(new[] { apiDocumentation.Object }).GetEnumerator());
            HttpCall.Setup(_ => _.HttpCall(ApiDocumentationUrl, It.IsAny<IHttpOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response = Return.Ok());
            HypermediaProcessor
                .Setup(_ => _.Process(It.IsAny<IResponse>(), It.IsAny<IHydraClient>(), It.IsAny<IHypermediaProcessingOptions>()))
                .ReturnsAsync(hypermedia.Object);
        }

        public override async Task TheTest()
        {
            await Client.GetApiDocumentation(BaseUrl);
        }

        [Test]
        public void should_call_the_given_site_url()
        {
            HttpCall.Verify(_ => _.HttpCall(BaseUrl, It.IsAny<IHttpOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Test]
        public void should_fetch_the_API_documentation()
        {
            HttpCall.Verify(
                _ => _.HttpCall(new Uri(BaseUrl, "api/documentation"), It.IsAny<IHttpOptions>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
                
        [Test]
        public void should_process_API_documentation_with_hypermedia_processor()
        {
            HypermediaProcessor.Verify(
                _ => _.Process(
                    Response,
                    Client,
                    It.Is<IHypermediaProcessingOptions>(options => 
                        options.AuxiliaryOriginalUrl == BaseUrl
                        && options.AuxiliaryResponse == UrlResponse
                        && options.LinksPolicy == LinksPolicy.Strict
                        && options.OriginalUrl == ApiDocumentationUrl)),
                Times.Once);
        }
    }
}
