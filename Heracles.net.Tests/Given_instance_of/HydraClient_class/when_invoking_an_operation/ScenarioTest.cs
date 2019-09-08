using Heracles.DataModel;
using Moq;
using RDeF.Entities;

namespace Given_instance_of.HydraClient_class.when_invoking_an_operation
{
    public abstract class ScenarioTest : HydraClientTest
    {
        protected static readonly Iri TargetIri = new Iri("some:iri");

        protected Mock<IOperation> Operation { get; private set; }

        protected Mock<IWebResource> Body { get; private set; }

        protected Mock<IResource> Parameters { get; private set; }

        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            var target = new Mock<IResource>(MockBehavior.Strict);
            target.SetupGet(_ => _.Iri).Returns(TargetIri);
            Operation = new Mock<IOperation>(MockBehavior.Strict);
            Operation.SetupGet(_ => _.Target).Returns(target.Object);
            Operation.SetupGet(_ => _.Method).Returns("GET");
            Body = new Mock<IWebResource>(MockBehavior.Strict);
            Parameters = new Mock<IResource>(MockBehavior.Strict);
            IriTemplateExpansionStrategy
                .Setup(_ => _.CreateRequest(It.IsAny<IOperation>(), It.IsAny<IResource>(), It.IsAny<IResource>()))
                .Returns(Operation.Object);
        }
    }
}
