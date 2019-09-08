using System.Collections.Generic;
using FluentAssertions;
using Heracles.DataModel;
using Heracles.DataModel.Collections;
using Heracles.Testing;
using Moq;
using NUnit.Framework;
using RDeF.Entities;

namespace Given_instance_of
{
    [TestFixture]
    public class Resource_collection
    {
        protected Mock<IResource> Resource1 { get; private set; }

        protected Mock<IResource> Resource2 { get; private set; }

        protected ICollection<IResource> Resources { get; private set; }

        [Test]
        public void should_provide_only_resources_of_type_specified()
        {
            Resources.OfType(new Iri("http://temp.uri/vocab#Class")).Should().BeEquivalentTo(Resource1.Object);
        }

        [Test]
        public void should_provide_only_non_blank_resources_of_type_specified()
        {
            Resources.OfType(new Iri("http://temp.uri/vocab#Class")).NonBlank().Should().BeEmpty();
        }

        [Test]
        public void should_provide_only_non_blank_resources()
        {
            Resources.NonBlank().Should().BeEquivalentTo(Resource2.Object);
        }

        [SetUp]
        public void Setup()
        {
            Resource1 = Resource.Of<IResource>(new Iri("_:blank"))
                .With(_ => _.Type, new HashSet<Iri>() { new Iri("http://temp.uri/vocab#Class") });
            Resource2 = Resource.Of<IResource>(new Iri("http://temp.uri/vocav#term"))
                .With(_ => _.Type, new HashSet<Iri>() { new Iri("http://temp.uri/vocab#AnotherClass") });
            Resources = new List<IResource>() { Resource1.Object, Resource2.Object };
        }
    }
}
