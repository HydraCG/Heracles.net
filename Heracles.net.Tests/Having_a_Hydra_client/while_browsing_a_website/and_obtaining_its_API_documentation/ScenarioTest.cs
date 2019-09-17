using FluentAssertions;
using Heracles.DataModel;
using Heracles.DataModel.Collections;
using Heracles.Namespaces;
using NUnit.Framework;
using RDeF.Entities;

namespace Having_a_Hydra_client.while_browsing_a_website.and_obtaining_its_API_documentation
{
    /// <summary>Use case 2. API documentation.</summary>
    public abstract class ScenarioTest : while_browsing_a_website.ScenarioTest
    {
        [Test]
        public void should_obtain_an_API_documentation()
        {
            ApiDocumentation.Awaiting(_ => _.GetEntryPoint()).Should().NotThrow();
        }

        [Test]
        public void should_have_access_an_entry_point()
        {

            ApiDocumentation.EntryPoint.Iri.ToString().Should().MatchRegex(".*/api$");
        }

        /// <summary>Use case 2.1. API documentation data structures.</summary>
        [Test]
        public void should_provide_class_of_schema_Event()
        {
            ApiDocumentation.SupportedClasses.OfIri(new Iri("http://schema.org/Event")).Should().BeEquivalentTo(
                new object[]
                {
                    new
                    {
                        Collections = new ICollection[0],
                        TextDescription = "An event happening at a certain time and location, such as a concert, lecture, or festival.",
                        DisplayName = "Event",
                        Iri = new Iri("http://schema.org/Event"),
                        Links = new ILink[0],
                        Operations = new IOperation[0],
                        SupportedOperations = new IOperation[0],
                        SupportedProperties = new object[]
                        {
                            new
                            {
                                Collections = new ICollection[0],
                                Iri = new Iri("_:b0"),
                                Links = new ILink[0],
                                Operations = new IOperation[0],
                                Property = new
                                {
                                    TextDescription = "The name of the event.",
                                    DisplayName = "Name",
                                    Iri = new Iri("http://schema.org/name"),
                                    Type = new[] { rdf.Property },
                                    ValuesOfType = new object[] { new { Iri = new Iri("http://www.w3.org/2001/XMLSchema#string"), Type = new Iri[0] } }
                                },
                                Readable = false,
                                Required = false,
                                Type = new[] { hydra.SupportedProperty },
                                Writable = false
                            },
                            new
                            {
                                Collections = new ICollection[0],
                                Iri = new Iri("_:b1"),
                                Links = new ILink[0],
                                Operations = new IOperation[0],
                                Property = new
                                {
                                    TextDescription = "Description of the event.",
                                    DisplayName = "Description",
                                    Iri = new Iri("http://schema.org/description"),
                                    Type = new[] { rdf.Property },
                                    ValuesOfType = new object[] { new { Iri = new Iri("http://www.w3.org/2001/XMLSchema#string"), Type = new Iri[0] } }
                                },
                                Readable = false,
                                Required = false,
                                Type = new[] { hydra.SupportedProperty },
                                Writable = false
                            },
                            new
                            {
                                Collections = new ICollection[0],
                                Iri = new Iri("_:b2"),
                                Links = new ILink[0],
                                Operations = new IOperation[0],
                                Property = new
                                {
                                    TextDescription = "The start date and time of the item (in ISO 8601 date format).",
                                    DisplayName = "Start date",
                                    Iri = new Iri("http://schema.org/startDate"),
                                    Type = new[] { rdf.Property },
                                    ValuesOfType = new object[]
                                    {
                                        new { Iri = new Iri("http://www.w3.org/2001/XMLSchema#dateTime"), Type = new Iri[0] },
                                        new { Iri = new Iri("http://www.w3.org/2001/XMLSchema#date"), Type = new Iri[0] }
                                    }
                                },
                                Readable = false,
                                Required = false,
                                Type = new[] { hydra.SupportedProperty },
                                Writable = false
                            },
                            new
                            {
                                Collections = new ICollection[0],
                                Iri = new Iri("_:b3"),
                                Links = new ILink[0],
                                Operations = new IOperation[0],
                                Property = new
                                {
                                    TextDescription = "The end date and time of the item (in ISO 8601 date format).",
                                    DisplayName = "End date",
                                    Iri = new Iri("http://schema.org/endDate"),
                                    Type = new[] { rdf.Property },
                                    ValuesOfType = new object[]
                                    {
                                        new { Iri = new Iri("http://www.w3.org/2001/XMLSchema#dateTime"), Type = new Iri[0] },
                                        new { Iri = new Iri("http://www.w3.org/2001/XMLSchema#date"), Type = new Iri[0] }
                                    }
                                },
                                Readable = false,
                                Required = false,
                                Type = new[] { hydra.SupportedProperty },
                                Writable = false
                            }
                        },
                        Type = new[] { hydra.Class }
                    }
                }
            );
        }
    }
}
