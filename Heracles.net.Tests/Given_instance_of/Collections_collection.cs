using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Heracles.DataModel;
using Heracles.DataModel.Collections;
using Heracles.Namespaces;
using Heracles.Testing;
using Moq;
using NUnit.Framework;

namespace Given_instance_of
{
    [TestFixture]
    public class Collections_collection
    {
        private Mock<IStatement> Statement1 { get; set; }

        private Mock<ICollection> Collection1 { get; set; }

        private Mock<IStatement> Statement2 { get; set; }

        private Mock<ICollection> Collection2 { get; set; }

        private ICollection<ICollection> Collection { get; set; }

        [Test]
        public void should_narrow_collection_to_those_describing_members_of_type_hydra_Class()
        {
            Collection.WithMembersOfType(hydra.Class).Should().HaveCount(1).And.Subject.First().Should().Be(Collection1.Object);
        }

        [Test]
        public void should_narrow_collection_to_those_describing_members_in_relation_of_schema_knows()
        {
            Collection.WithMembersInRelationWith(gh.alien_mcl, schema.knows).Should().HaveCount(1).And.Subject.First().Should().Be(Collection2.Object);
        }

        [SetUp]
        public void Setup()
        {
            Statement1 = new Mock<IStatement>()
                .With(_ => _.Property, Resource.Of<IResource>(rdf.type).Object)
                .With(_ => _.Object, Resource.Of<IResource>(hydra.Class).Object);
            Collection1 = Resource.Of<ICollection>().With(_ => _.Manages, new HashSet<IStatement>() { Statement1.Object });
            Statement2 = new Mock<IStatement>()
                .With(_ => _.Subject, Resource.Of<IResource>(gh.alien_mcl).Object)
                .With(_ => _.Property, Resource.Of<IResource>(schema.knows).Object);
            Collection2 = Resource.Of<ICollection>().With(_ => _.Manages, new HashSet<IStatement>() { Statement2.Object });
            Collection = new List<ICollection>() { Collection1.Object, Collection2.Object };
        }
    }
}
