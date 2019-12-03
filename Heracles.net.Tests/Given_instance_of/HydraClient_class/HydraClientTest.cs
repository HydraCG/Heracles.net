using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles;
using Heracles.Testing;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.HydraClient_class
{
    public abstract class HydraClientTest
    {
        protected static readonly Uri BaseUrl = new Uri("http://temp.uri/");

        protected Mock<IHypermediaProcessor> HypermediaProcessor { get; private set; }

        protected Mock<IIriTemplateExpansionStrategy> IriTemplateExpansionStrategy { get; private set; }

        protected Mock<IHttpInfrastructure> HttpCall { get; private set; }

        protected HydraClient Client { get; private set; }

        public virtual Task TheTest()
        {
            return Task.CompletedTask;
        }

        [Test]
        public void should_register_a_hypermedia_processor()
        {
            Client.GetHypermediaProcessor(Return.Ok()).Should().Be(HypermediaProcessor.Object);
        }

        [SetUp]
        public async Task Setup()
        {
            HypermediaProcessor = new Mock<IHypermediaProcessor>(MockBehavior.Strict);
            HypermediaProcessor.Setup(_ => _.Supports(It.IsAny<IResponse>()))
                .Returns<IResponse>(_ => _.Headers["Content-Type"].Contains("application/ld+json") ? Level.FullSupport : Level.None);
            IriTemplateExpansionStrategy = new Mock<IIriTemplateExpansionStrategy>(MockBehavior.Strict);
            HttpCall = new Mock<IHttpInfrastructure>(MockBehavior.Strict);
            HttpCall.Setup(_ => _.HttpCall(It.IsAny<Uri>(), It.IsAny<IHttpOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Return.Ok());
            Client = new HydraClient(
                new[] { HypermediaProcessor.Object },
                IriTemplateExpansionStrategy.Object,
                LinksPolicy.Strict,
                HttpCall.Object.HttpCall);
            ScenarioSetup();
            await TheTest();
        }

        public virtual void ScenarioSetup()
        {
        }

        protected IDictionary<string, IEnumerable<string>> WithHeaders(string name, string value)
        {
            return new Dictionary<string, IEnumerable<string>>() { { name, new[] { value } } };
        }
    }
}
