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
    public class Mappings_collection
    {
        protected Mock<IIriTemplateMapping> Mapping1 { get; private set; }

        protected Mock<IIriTemplateMapping> Mapping2 { get; private set; }

        protected ICollection<IIriTemplateMapping> Mappings { get; private set; }

        [Test]
        public void should_provide_only_variable_name_matching_mappings()
        {
            Mappings.OfVariableName("variable1").Should().BeEquivalentTo(Mapping1.Object);
        }

        [SetUp]
        public void Setup()
        {
            Mapping1 = Resource.Of<IIriTemplateMapping>()
                .With(_ => _.Type, new HashSet<Iri>() { hydra.IriTemplateMapping })
                .With(_ => _.Variable, "variable1");
            Mapping2 = Resource.Of<IIriTemplateMapping>()
                .With(_ => _.Type, new HashSet<Iri>() { hydra.IriTemplateMapping })
                .With(_ => _.Variable, "variable2");
            Mappings = new List<IIriTemplateMapping>() { Mapping1.Object, Mapping2.Object };
        }
    }
}
