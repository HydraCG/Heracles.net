using System;
using System.Collections.Generic;
using FluentAssertions;
using Heracles.DataModel;
using Heracles.JsonLd;
using Heracles.Namespaces;
using Moq;
using NUnit.Framework;
using RDeF.Entities;
using RollerCaster;

namespace Given_instance_of.TemplatedOperation_interface
{
    [TestFixture]
    public class when_expanding_URI_with_variable_values
    {
        private static readonly Iri AddAction = new Iri("http://schema.org/AddAction");

        private Mock<IIriTemplate> Template { get; set; }

        private ITemplatedOperation Operation { get; set; }

        private IOperation Result { get; set; }

        public void TheTest()
        {
            Result = Operation.ExpandTarget(new Dictionary<string, string>() { { "with_variable", "test-value" } });
        }

        [Test]
        public void should_provide_an_expanded_URL()
        {
            Result.Target.Iri.Should().Be(new Iri("http://temp.uri/some-uri?with_variable=test-value"));
        }

        [Test]
        public void should_pass_a_correct_method()
        {
            Result.Method.Should().Be(Operation.Method);
        }

        [Test]
        public void should_copy_original_operations_types()
        {
            Result.Type.Should().BeEquivalentTo(AddAction, hydra.Operation);
        }

        [SetUp]
        public void Setup()
        {
            Template = new Mock<IIriTemplate>(MockBehavior.Strict);
            Template.SetupGet(_ => _.Template).Returns("some-uri{?with_variable}");
            var target = new Mock<IResource>(MockBehavior.Strict);
            target.SetupGet(_ => _.Iri).Returns(new Iri("test-url"));
            var context = new Mock<IEntityContext>(MockBehavior.Strict);
            context.Setup(_ => _.Create<IOperation>(It.IsAny<Iri>())).Returns<Iri>(Create<IOperation>);
            context.Setup(_ => _.Create<IResource>(It.IsAny<Iri>())).Returns<Iri>(Create<IResource>);
            var proxy = new MulticastObject();
            proxy.SetProperty(typeof(IPointingResource).GetProperty(nameof(IPointingResource.BaseUrl)), new Uri("http://temp.uri/"));
            proxy.SetProperty(typeof(IPointingResource).GetProperty(nameof(IPointingResource.Target)), target.Object);
            proxy.SetProperty(JsonLdHypermediaProcessor.IriTemplatePropertyInfo, Template.Object);
            proxy.SetProperty(typeof(IEntity).GetProperty(nameof(IEntity.Context)), context.Object);
            Operation = proxy.ActLike<ITemplatedOperation>();
            Operation.Type.Add(AddAction);
            Operation.Type.Add(hydra.Operation);
            TheTest();
        }

        private T Create<T>(Iri iri)
        {
            var result = new MulticastObject();
            result.SetProperty(typeof(IEntity).GetProperty(nameof(IEntity.Iri)), iri);
            return result.ActLike<T>();
        }
    }
}
