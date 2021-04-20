using System;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Moq;
using NUnit.Framework;

namespace Given_instance_of
{
    [TestFixture]
    public class HydraClientFactory_class
    {
        protected HydraClientFactory Factory { get; private set; }

        protected Mock<IHypermediaProcessor> Processor { get; private set; }

        protected Mock<IResponse> Response { get; private set; }

        [SetUp]
        public void Setup()
        {
            Factory = HydraClientFactory.Configure().WithDefaults();
            Processor = new Mock<IHypermediaProcessor>(MockBehavior.Strict);
            Processor.Setup(_ => _.Supports(It.IsAny<IResponse>())).Returns(Level.FullSupport);
            var headers = new Mock<IHeaders>(MockBehavior.Strict);
            headers.SetupGet(_ => _[It.IsAny<string>()]).Returns(Array.Empty<string>());
            Response = new Mock<IResponse>(MockBehavior.Strict);
            Response.SetupGet(_ => _.Headers).Returns(headers.Object);
        }

        [Test]
        public void should_create_a_client_interpreting_all_relations_as_links()
        {
            Factory.WithAllLinks().AndCreate().Should().BeOfType<HydraClient>()
                .Which.LinksPolicy.Should().Be(LinksPolicy.All);
        }

        [Test]
        public void should_create_a_client_interpreting_all_HTTP_related_resources_as_links()
        {
            Factory.WithAllHttpLinks().AndCreate().Should().BeOfType<HydraClient>()
                .Which.LinksPolicy.Should().Be(LinksPolicy.AllHttp);
        }

        [Test]
        public void should_create_a_client_interpreting_all_related_resources_with_same_root_URL_as_links()
        {
            Factory.WithSameRootLinks().AndCreate().Should().BeOfType<HydraClient>()
                .Which.LinksPolicy.Should().Be(LinksPolicy.SameRoot);
        }

        [Test]
        public void should_create_a_client_fetching_no_API_documentations()
        {
            Factory.WithNoApiDocumentations().AndCreate().Should().BeOfType<HydraClient>()
                .Which.ApiDocumentationPolicy.Should().Be(ApiDocumentationPolicy.None);
        }

        [Test]
        public void should_create_a_client_fetching_API_documentations()
        {
            Factory.WithApiDocumentationsFetchedOnly().AndCreate().Should().BeOfType<HydraClient>()
                .Which.ApiDocumentationPolicy.Should().Be(ApiDocumentationPolicy.FetchOnly);
        }

        [Test]
        public void should_create_a_client_fetching_and_extending_API_documentations()
        {
            Factory.WithApiDocumentationsFetchedAndExtended().AndCreate().Should().BeOfType<HydraClient>()
                .Which.ApiDocumentationPolicy.Should().Be(ApiDocumentationPolicy.FetchAndExtend);
        }

        [Test]
        public void should_create_a_client_with_custom_hypermedia_processor()
        {
            Factory.With(Processor.Object).AndCreate().GetHypermediaProcessor(Response.Object)
                .Should().Be(Processor.Object);
        }
    }
}
