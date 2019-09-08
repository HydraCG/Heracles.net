using System;
using System.Collections.Generic;
using FluentAssertions;
using Heracles.DataModel;
using Moq;
using NUnit.Framework;
using RDeF.Entities;

namespace Given_instance_of.MappingsBuilder_class
{
    [TestFixture]
    public class when_defining_mappings
    {
        protected IIriTemplateMapping[] Mappings { get; private set; }

        protected MappingsBuilder Builder { get; private set; }

        protected IDictionary<string, string> MappedVariables { get; private set; }

        public void TheTest()
        {
            MappedVariables = Builder
                .WithProperty(new Iri("http://schema.org/name")).HavingValueOf("name")
                .WithVariable("eventDescription").HavingValueOf("description")
                .Complete();
        }

        [Test]
        public void should_map_value_by_property_correctly()
        {
            MappedVariables.Should().ContainKey("eventName").WhichValue.Should().Be("name");
        }

        [Test]
        public void should_map_value_by_variable_name_correctly()
        {
            MappedVariables.Should().ContainKey("eventDescription").WhichValue.Should().Be("description");
        }

        [Test]
        public void should_throw_on_attempt_to_map_unknown_property()
        {
            Builder.Invoking(_ => _.WithProperty(new Iri("http://unknown.property/"))).Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void should_throw_on_attempt_to_map_unknown_variable()
        {
            Builder.Invoking(_ => _.WithVariable("unknownVariable")).Should().Throw<InvalidOperationException>();
        }

        [SetUp]
        public void Setup()
        {
            Mappings = new[]
            {
                TemplateMappingOf(new Iri("http://schema.org/name"), "eventName"),
                TemplateMappingOf(new Iri("http://schema.org/description"), "eventDescription")
            };
            Builder = new MappingsBuilder(Mappings);
            TheTest();
        }

        private IIriTemplateMapping TemplateMappingOf(Iri iri, string variableName)
        {
            var property = new Mock<IResource>(MockBehavior.Strict);
            property.SetupGet(_ => _.Iri).Returns(iri);
            var result = new Mock<IIriTemplateMapping>(MockBehavior.Strict);
            result.SetupGet(_ => _.Property).Returns(property.Object);
            result.SetupGet(_ => _.Variable).Returns(variableName);
            return result.Object;
        }
    }
}
