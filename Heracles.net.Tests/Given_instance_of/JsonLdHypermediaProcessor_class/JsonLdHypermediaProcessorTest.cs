using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.JsonLd;
using Heracles.Testing;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.JsonLdHypermediaProcessor_class
{
    public abstract class JsonLdHypermediaProcessorTest
    {

        protected Mock<IOntologyProvider> OntologyProvider { get; private set; }

        protected Mock<IHttpInfrastructure> HttpCall { get; private set; }

        protected JsonLdHypermediaProcessor HypermediaProcessor { get; private set; }

        protected Mock<IHydraClient> Client { get; private set; }

        protected Mock<IResponse> Response { get; private set; }

        protected Mock<IHeaders> ResponseHeaders { get; private set; }

        protected IHypermediaContainer Result { get; private set; }

        public virtual async Task TheTest()
        {
            Result = await HypermediaProcessor.Process(Response.Object, Client.Object);
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
            HypermediaProcessor = new JsonLdHypermediaProcessor(OntologyProvider.Object, HttpCall.Object.HttpCall);
            Client = new Mock<IHydraClient>(MockBehavior.Strict);
            ResponseHeaders = new Mock<IHeaders>(MockBehavior.Strict);
            Response = new Mock<IResponse>(MockBehavior.Strict);
            Response.SetupGet(_ => _.Headers).Returns(ResponseHeaders.Object);
            ScenarioSetup();
            await TheTest();
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

        protected virtual void ScenarioSetup()
        {
        }
    }
}
