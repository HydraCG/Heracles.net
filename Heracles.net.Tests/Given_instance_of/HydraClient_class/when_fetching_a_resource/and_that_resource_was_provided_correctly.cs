using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.Testing;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.HydraClient_class.when_fetching_a_resource
{
    [TestFixture]
    public class and_that_resource_was_provided_correctly : ScenarioTest
    {
        private Mock<IHypermediaContainer> Resource { get; set; }

        private IResponse ResourceResponse { get; set; }

        private IHypermediaContainer Result { get; set; }

        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            Resource = new Mock<IHypermediaContainer>(MockBehavior.Strict);
            HttpCall.Setup(_ => _.HttpCall(ResourceUrl, It.IsAny<IHttpOptions>()))
                .ReturnsAsync(ResourceResponse = Return.Ok(
                    ResourceUrl,
                    new MemoryStream(),
                    WithHeaders("Content-Type", "application/ld+json")));
            HypermediaProcessor
                .Setup(_ => _.Process(
                    It.IsAny<IResponse>(),
                    It.IsAny<IHydraClient>(),
                    It.IsAny<IHypermediaProcessingOptions>()))
                .ReturnsAsync(Resource.Object);
        }
        
        public override async Task TheTest()
        {
            Result = await Client.GetResource(ResourceUrl);
        }

        [Test]
        public void should_process_the_response()
        {
            HypermediaProcessor.Verify(
                _ => _.Process(ResourceResponse, Client, It.IsAny<IHypermediaProcessingOptions>()),
                Times.Once);
        }

        [Test]
        public void should_return_a_correct_result()
        {
            Result.As<object>().Should().Be(Resource.Object);
        }
    }
}
