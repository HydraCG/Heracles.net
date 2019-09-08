using System;
using System.Collections.Generic;
using FluentAssertions;
using Heracles.DataModel;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.BodyResourceBoundIriTemplateExpansionStrategy_class
{
    [TestFixture]
    public class when_handling_a_templated_operation : BodyResourceBoundIriTemplateExpansionStrategyTest
    {
        private Mock<ITemplatedOperation> Operation { get; set; }

        private IDictionary<string, string> Variables { get; set; }

        private IOperation Result { get; set; }

        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            Operation = new Mock<ITemplatedOperation>(MockBehavior.Strict);
            Operation.Setup(_ => _.ExpandTarget(It.IsAny<Action<MappingsBuilder>>()))
                .Returns<Action<MappingsBuilder>>(_ =>
                {
                    _(Builder);
                    Variables = Builder.Complete();
                    return new Mock<IOperation>().Object;
                });
        }

        public override void TheTest()
        {
            Result = Strategy.CreateRequest(Operation.Object, Body, Parameters);
        }

        [Test]
        public void should_expand_target()
        {
            Operation.Verify(_ => _.ExpandTarget(It.IsAny<Action<MappingsBuilder>>()), Times.Once);
        }

        [Test]
        public void should_use_mappings_from_the_body_resource()
        {
            Variables["eventDescription"].Should().Be(Body.Description);
        }

        [Test]
        public void should_use_mappings_from_the_auxiliary_resource()
        {
            Variables["eventName"].Should().Be(Parameters.Name);
        }
    }
}
