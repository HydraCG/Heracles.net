using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.JsonLd;
using Heracles.Rdf;
using Heracles.Rdf.GraphTransformations;
using Heracles.Testing;
using Moq;
using NUnit.Framework;
using RDeF.Entities;
using VDS.RDF;

namespace Given_instance_of.JsonLdHypermediaProcessor_class
{
    public abstract class JsonLdHypermediaProcessorTest
    {
        protected Mock<IOntologyProvider> OntologyProvider { get; private set; }

        protected Mock<IHttpInfrastructure> HttpCall { get; private set; }
        
        protected Mock<IGraphTransformer> GraphTransformer { get; private set; }

        protected JsonLdHypermediaProcessor HypermediaProcessor { get; private set; }

        protected Mock<IHydraClient> Client { get; private set; }

        protected Mock<IResponse> Response { get; private set; }

        protected Mock<IHeaders> ResponseHeaders { get; private set; }

        protected IHypermediaContainer Result { get; private set; }
        
        protected Mock<IHypermediaProcessingOptions> HypermediaProcessingOptions { get; private set; }
        
        protected abstract Uri Uri { get; }

        public virtual async Task TheTest()
        {
            Result = await HypermediaProcessor.Process(
                Response.Object,
                Client.Object,
                HypermediaProcessingOptions.Object);
        }

        [Test]
        public void should_expose_supported_media_types()
        {
            HypermediaProcessor.SupportedMediaTypes.Should().BeEquivalentTo("application/ld+json", "application/json");
        }
        
        [SetUp]
        public async Task Setup()
        {
            OntologyProvider = new Mock<IOntologyProvider>(MockBehavior.Strict);
            HttpCall = new Mock<IHttpInfrastructure>(MockBehavior.Strict);
            GraphTransformer = new Mock<IGraphTransformer>(MockBehavior.Strict);
            GraphTransformer.Setup(
                    _ => _.Transform(
                        It.IsAny<IEnumerable<ITypedEntity>>(),
                        It.IsAny<IHypermediaProcessor>(),
                        It.IsAny<IHypermediaProcessingOptions>()))
                .Returns<IEnumerable<ITypedEntity>, IHypermediaProcessor, IHypermediaProcessingOptions>(
                    (resources, processor, options) => resources);
            HypermediaProcessor = new JsonLdHypermediaProcessor(
                OntologyProvider.Object,
                HttpCall.Object.HttpCall,
                GraphTransformer.Object);
            Client = new Mock<IHydraClient>(MockBehavior.Strict);
            ResponseHeaders = new Mock<IHeaders>(MockBehavior.Strict);
            Response = new Mock<IResponse>(MockBehavior.Strict);
            Response.SetupGet(_ => _.Headers).Returns(ResponseHeaders.Object);
            HypermediaProcessingOptions = new Mock<IHypermediaProcessingOptions>(MockBehavior.Strict);
            HypermediaProcessingOptions.SetupGet(_ => _.LinksPolicy).Returns(LinksPolicy.Strict);
            HypermediaProcessingOptions.SetupGet(_ => _.ApiDocumentationPolicy).Returns(ApiDocumentationPolicy.None);
            HypermediaProcessingOptions.SetupGet(_ => _.ApiDocumentations).Returns(Array.Empty<IApiDocumentation>());
            HypermediaProcessingOptions.SetupGet(_ => _.OriginalUrl).Returns(Uri);
            HypermediaProcessingOptions.SetupGet(_ => _.AuxiliaryOriginalUrl).Returns((Uri)null);
            HypermediaProcessingOptions.SetupGet(_ => _.AuxiliaryResponse).Returns((IResponse)null);
            ScenarioSetup();
            await TheTest();
        }

        public virtual void ScenarioSetup()
        {
        }

        protected static Stream GetResourceNamed(string resourceName)
        {
            var result = (
                from resource in typeof(JsonLdHypermediaProcessorTest).Assembly.GetManifestResourceNames()
                where resource.EndsWith(resourceName)
                select typeof(JsonLdHypermediaProcessorTest).Assembly.GetManifestResourceStream(resource)).First();
            result.Seek(0, SeekOrigin.Begin);
            return result;
        }
    }
}
