using System.Threading;
using System.Threading.Tasks;
using Heracles;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.HydraClient_class.when_invoking_an_operation
{
    [TestFixture]
    public class and_valid_operation_was_provided : ScenarioTest
    {
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
        public void should_execute_the_request()
        {
            HttpCall.Verify(_ => _.HttpCall(TargetIri, It.IsAny<IHttpOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
