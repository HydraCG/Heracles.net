using FluentAssertions;
using Heracles.DataModel;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.BodyResourceBoundIriTemplateExpansionStrategy_class
{
    [TestFixture]
    public class when_handling_a_standard_operation_without_a_template : BodyResourceBoundIriTemplateExpansionStrategyTest
    {
        private Mock<IOperation> Operation { get; set; }

        public override void ScenarioSetup()
        {
            base.ScenarioSetup();
            Operation = new Mock<IOperation>(MockBehavior.Strict);
        }

        [Test]
        public void should_provide_an_expanded_operation()
        {
            Strategy.CreateRequest(Operation.Object, Body, Parameters).Should().Be(Operation.Object);
        }
    }
}
