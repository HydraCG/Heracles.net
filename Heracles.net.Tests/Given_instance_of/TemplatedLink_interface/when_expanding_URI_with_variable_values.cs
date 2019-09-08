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

namespace Given_instance_of.TemplatedLink_interface
{
    [TestFixture]
    public class when_expanding_URI_with_variable_values
    {
        private Mock<IIriTemplate> Template { get; set; }

        private ITemplatedLink Link { get; set; }

        private ILink Result { get; set; }

        public void TheTest()
        {
            Result = Link.ExpandTarget(new Dictionary<string, string>() { { "with_variable", "test-value" } });
        }

        [Test]
        public void should_provide_an_expanded_URL()
        {
            Result.Target.Iri.Should().Be(new Iri("http://temp.uri/some-uri?with_variable=test-value"));
        }

        [Test]
        public void should_copy_original_operations_types()
        {
            Result.Type.Should().BeEquivalentTo(hydra.Link);
        }

        [SetUp]
        public void Setup()
        {
            Template = new Mock<IIriTemplate>(MockBehavior.Strict);
            Template.SetupGet(_ => _.Template).Returns("some-uri{?with_variable}");
            var target = new Mock<IResource>(MockBehavior.Strict);
            target.SetupGet(_ => _.Iri).Returns(new Iri("test-url"));
            var context = new Mock<IEntityContext>(MockBehavior.Strict);
            context.Setup(_ => _.Create<ILink>(It.IsAny<Iri>())).Returns<Iri>(Create<ILink>);
            context.Setup(_ => _.Create<IResource>(It.IsAny<Iri>())).Returns<Iri>(Create<IResource>);
            var proxy = new MulticastObject();
            proxy.SetProperty(typeof(IPointingResource).GetProperty(nameof(IPointingResource.BaseUrl)), new Uri("http://temp.uri/"));
            proxy.SetProperty(typeof(IPointingResource).GetProperty(nameof(IPointingResource.Target)), target.Object);
            proxy.SetProperty(typeof(IDerefencableLink).GetProperty(nameof(IDerefencableLink.Relation)), "test");
            proxy.SetProperty(JsonLdHypermediaProcessor.IriTemplatePropertyInfo, Template.Object);
            proxy.SetProperty(typeof(IEntity).GetProperty(nameof(IEntity.Context)), context.Object);
            Link = proxy.ActLike<ITemplatedLink>();
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
