using System.Collections.Generic;
using FluentAssertions;
using Heracles.DataModel;
using Heracles.DataModel.Collections;
using Heracles.Namespaces;
using Heracles.Testing;
using Moq;
using NUnit.Framework;
using RDeF.Entities;

namespace Given_instance_of
{
    [TestFixture]
    public class Links_collection
    {
        private static readonly Mock<IResource> Target = Resource.Of<IResource>(new Iri("some:resource"));

        protected Mock<ILink> Link1 { get; private set; }

        protected Mock<ITemplatedLink> Link2 { get; private set; }

        protected Mock<ILink> Link3 { get; private set; }

        protected Mock<ILink> Link4 { get; private set; }

        protected ICollection<IDereferencableLink> Links { get; private set; }

        [Test]
        public void should_provide_only_links_matching_required_relation()
        {
            Links.WithRelationOf(new Iri("yet:another-url")).Should().BeEquivalentTo(Link3.Object);
        }

        [Test]
        public void should_provide_only_links_with_template()
        {
            Links.WithTemplate().Should().BeEquivalentTo(Link2.Object);
        }

        [SetUp]
        public void Setup()
        {
            Link1 = Resource.Of<ILink>()
                .With(_ => _.Relation, "some:resource-url")
                .With<ILink, IPointingResource, IResource>(_ => _.Target, Target.Object)
                .With(_ => _.Type, new HashSet<Iri>() { hydra.Link });
            Link2 = Resource.Of<ITemplatedLink>()
                .With(_ => _.Relation, "some:other-url")
                .With<ITemplatedLink, IPointingResource, IResource>(_ => _.Target, Target.Object)
                .With(_ => _.Type, new HashSet<Iri>() { hydra.TemplatedLink });
            Link3 = Resource.Of<ILink>()
                .With(_ => _.Relation, "yet:another-url")
                .With<ILink, IPointingResource, IResource>(_ => _.Target, Target.Object)
                .With(_ => _.Type, new HashSet<Iri>() { hydra.Link });
            Link4 = Resource.Of<ILink>()
                .With(_ => _.Relation, "yet:another-other-url")
                .With<ILink, IPointingResource, IResource>(_ => _.Target, Target.Object)
                .With(_ => _.Type, new HashSet<Iri>() { hydra.Link });
            Links = new List<IDereferencableLink>() { Link1.Object, Link2.Object, Link3.Object, Link4.Object };
        }
    }
}
