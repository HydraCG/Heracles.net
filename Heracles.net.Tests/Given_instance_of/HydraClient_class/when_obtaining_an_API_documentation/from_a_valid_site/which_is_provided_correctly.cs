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
        
        private Mock<IApiDocumentation> ApiDocumentation { get; set; }

        public override void ScenarioSetup()
        {
            base.ScenarioSetup();
            ApiDocumentationUrl = new Uri(BaseUrl, "api/documentation");
            ApiDocumentation = new Mock<IApiDocumentation>(MockBehavior.Strict);
            ApiDocumentation.SetupGet(_ => _.EntryPoint).Returns(Resource.Of<IResource>(new Uri(BaseUrl, "api")).Object);
            ApiDocumentation.SetupGet(_ => _.Type).Returns(new HashSet<Iri>() { hydra.ApiDocumentation });
            var hypermedia = new Mock<IHypermediaContainer>(MockBehavior.Strict);
            hypermedia.Setup(_ => _.GetEnumerator())
                .Returns(new List<IResource>(new[] { ApiDocumentation.Object }).GetEnumerator());
            HttpCall.Setup(_ => _.HttpCall(ApiDocumentationUrl, It.IsAny<IHttpOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response = Return.Ok());
            HypermediaProcessor
                .Setup(_ => _.Process(
                    It.IsAny<IResponse>(),
                    It.IsAny<IHydraClient>(),
                    It.IsAny<IHypermediaProcessingOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(hypermedia.Object);
            Cache.Setup(_ => _.All<IApiDocumentation>()).Returns(Array.Empty<IApiDocumentation>());
            Cache.SetupSet(_ => _[It.IsAny<Uri>()] = It.IsAny<IResource>());
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
                _ => _.HttpCall(ApiDocumentationUrl, It.IsAny<IHttpOptions>(), It.IsAny<CancellationToken>()),
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
                        && options.OriginalUrl == ApiDocumentationUrl),
                    CancellationToken.None),
                Times.Once);
        }

        [Test]
        public void should_try_obtaining_an_API_documentation_from_cache()
        {
            Cache.VerifyGet(_ => _[ApiDocumentationUrl], Times.Once);
        }

        [Test]
        public void should_store_obtained_API_documentation_in_cache()
        {
            Cache.VerifySet(_ => _[ApiDocumentationUrl] = ApiDocumentation.Object, Times.Once);
        }

        [Test]
        public void should_obtain_all_cached_API_documentations_so_far()
        {
            Cache.Verify(_ => _.All<IApiDocumentation>(), Times.Once);
        }
    }
}
