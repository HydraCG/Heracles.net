using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Heracles;
using Heracles.DataModel;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.HydraClient_class.when_invoking_an_operation
{
    [TestFixture]
    public class and_valid_operation_was_provided : ScenarioTest
    {
        private static readonly Stream SerializedBody = new MemoryStream();

        public override async Task TheTest()
        {
            await Client.Invoke(Operation.Object, Body.Object, Parameters.Object);
        }

        [Test]
        public void should_create_a_request_operation_with_the_strategy_provided()
        {
            IriTemplateExpansionStrategy.Verify(
                _ => _.CreateRequest(Operation.Object, Body.Object, Parameters.Object),
                Times.Once);
        }

        [Test]
        public void should_serialize_body()
        {
            HypermediaProcessor.Verify(_ => _.Serialize(Body.Object, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void should_execute_the_request()
        {
            HttpCall.Verify(_ => _.HttpCall(TargetIri, It.IsAny<IHttpOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void should_send_the_serialized_body()
        {
            HttpCall.Verify(
                _ => _.HttpCall(It.IsAny<Uri>(), It.Is<IHttpOptions>(options => options.Body == SerializedBody), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            HypermediaProcessor.SetupGet(_ => _.SupportedMediaTypes).Returns(new[] { "application/ld+json" });
            HypermediaProcessor.Setup(_ => _.Serialize(It.IsAny<IResource>(), It.IsAny<CancellationToken>())).ReturnsAsync(SerializedBody);
        }
    }
}
