using System;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.Rdf;
using Heracles.Testing;
using Moq;
using NUnit.Framework;
using RollerCaster;

namespace Given_instance_of.IApiDocumentation_interface
{
    [TestFixture]
    public class when_obtaining_an_entry_point
    {
        private Mock<IHydraClient> Client { get; set; }

        private IApiDocumentation ApiDocumentation { get; set; }

        private Mock<IHypermediaContainer> EntryPoint { get; set; }

        private IHypermediaContainer Result { get; set; }

        [SetUp]
        public async Task Setup()
        {
            EntryPoint = new Mock<IHypermediaContainer>(MockBehavior.Strict);
            Client = new Mock<IHydraClient>(MockBehavior.Strict);
            Client.Setup(_ => _.GetResource(It.IsAny<IResource>())).ReturnsAsync(EntryPoint.Object);
            var proxy = new MulticastObject();
            proxy.SetProperty(HypermediaProcessorBase.ClientPropertyInfo, Client.Object);
            ApiDocumentation = proxy.ActLike<IApiDocumentation>();
            ApiDocumentation.EntryPoint = Resource.Of<IResource>(new Uri("http://temp.uri/api")).Object;
            await TheTest();
        }

        public async Task TheTest()
        {
            Result = await ApiDocumentation.GetEntryPoint();
        }

        [Test]
        public void should_call_the_client()
        {
            Client.Verify(_ => _.GetResource(ApiDocumentation.EntryPoint));
        }

        [Test]
        public void should_provide_a_correct_result()
        {
            Result.As<object>().Should().Be(EntryPoint.Object);
        }
    }
}
