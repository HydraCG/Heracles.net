using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Heracles.DataModel.Collections;
using Heracles.Namespaces;
using NUnit.Framework;
using RDeF.Entities;
using RollerCaster;

namespace Given_instance_of.JsonLdHypermediaProcessor_class.when_parsing.JSON_LD_response
{
    [TestFixture]
    public class with_collections : ScenarioTest
    {
        private Stream _inputJsonLd;

        protected override Stream InputJsonLd
        {
            get { return _inputJsonLd; }
        }

        protected override Uri Uri { get; } = new Uri("http://temp.uri/api", UriKind.Absolute);
        
        public override void ScenarioSetup()
        {
            _inputJsonLd = GetResourceNamed("collectionsInput.json");
            Response.SetupGet(_ => _.Url).Returns(Uri);
            base.ScenarioSetup();
        }

        [Test]
        public void should_trim_collections_expecting_schem_Person_to_people()
        {
            var proxy = Result.Collections.First().Manages.First().Unwrap();
            foreach (var property in proxy.PropertyValues)
            {
                Console.WriteLine($"{property.Property.DeclaringType}.{property.Property.Name}: {property.Value}");
            }

            Result.Collections.First().Manages.First().Object.Iri.Should().Be(schema.Person);
            Result.Collections.First().Manages.First().Property.Iri.Should().Be(rdf.type);
            Result.Collections.WithMembersOfType(schema.Person).First().Iri
                .Should().Be(new Iri("http://temp.uri/api/people"));
        }
        
        [Test]
        public void should_trim_collections_expecting_schema_person_to_known()
        {
            Result.Collections.WithMembersInRelationWith(Api.People.Karol, schema.knows).First().Iri
                .Should().Be(new Iri("http://temp.uri/api/people/karol/knows"));
        }
    }
}
