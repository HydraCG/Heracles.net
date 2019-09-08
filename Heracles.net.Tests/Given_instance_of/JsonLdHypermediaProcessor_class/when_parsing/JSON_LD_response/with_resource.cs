using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Heracles.DataModel;
using Heracles.Namespaces;
using Heracles.Testing;
using NUnit.Framework;
using RDeF.Entities;

namespace Given_instance_of.JsonLdHypermediaProcessor_class.when_parsing.JSON_LD_response
{
    [TestFixture]
    public class with_resource : ScenarioTest
    {
        private Stream _inputJsonLd;

        protected override Stream InputJsonLd
        {
            get { return _inputJsonLd; }
        }

        [Test]
        public void should_process_data()
        {
            Result.GetBody().Result.Should().Be(InputJsonLd);
        }

        [Test]
        public void should_discover_all_collections()
        {
            Result.Collections.Count.Should().Be(2);
        }

        [Test]
        public void should_discover_people_collection()
        {
            Result.Collections.Should().Contain(new Regex("/api/people$"));
        }

        [Test]
        public void should_discover_events_collection()
        {
            Result.Collections.Should().Contain(new Regex("/api/events$"));
        }

        [Test]
        public void should_provide_response_headers()
        {
            Result.Headers["Content-Type"].Should().HaveCount(1)
                .And.Subject.First().Should().Be("application/ld+json");
        }

        [Test]
        public void should_separete_hypermedia()
        {
            Result.Should().BeEquivalentTo(
                new[]
                {
                    new
                    {
                        Collections = new object[]
                        {
                            new
                            {
                                Collections = new ICollection[0],
                                Iri = new Iri("http://temp.uri/api/people"),
                                Links = new ILink[0],
                                Manages = new IStatement[0],
                                Members = new IResource[0],
                                Operations = new IOperation[0],
                                TotalItems = 0,
                                Type = new[] { hydra.Collection }
                            },
                            new
                            {
                                Collections = new ICollection[0],
                                Iri = new Iri("http://temp.uri/api/events"),
                                Links = new object[]
                                {
                                    new
                                    {
                                        BaseUrl = "http://temp.uri/api",
                                        Collections = new ICollection[0],
                                        Iri = new Iri("http://temp.uri/api/events/closed"),
                                        Links = new ILink[0],
                                        Operations = new IOperation[0],
                                        Relation = new Iri("http://temp.uri/vocab/closed-events"),
                                        SupportedOperations = new IOperation[0],
                                        Target = new { Iri = new Iri("http://temp.uri/api/events/closed"), Type = new Iri[0] },
                                        Type = new[] { hydra.Link }
                                    },
                                    new
                                    {
                                        BaseUrl = "http://temp.uri/api",
                                        Collections = new ICollection[0],
                                        Iri = new Iri("_:b0"),
                                        Links = new ILink[0],
                                        Mappings = new object[]
                                        {
                                            new
                                            {
                                                Collections = new ICollection[0],
                                                Iri = new Iri("_:b1"),
                                                Links = new ILink[0],
                                                Operations = new IOperation[0],
                                                Property = new
                                                {
                                                    Collections = new ICollection[0],
                                                    Description = "",
                                                    DisplayName = "",
                                                    Iri = hydra.freetextQuery,
                                                    Links = new ILink[0],
                                                    Operations = new IOperation[0],
                                                    Type = new Iri[0],
                                                    valuesOfType = new Iri[0]
                                                },
                                                Required = false,
                                                Type = new Iri[] { hydra.IriTemplateMapping },
                                                Variable = "searchPhrase",
                                                VariableRepresentation = new
                                                {
                                                    Collections = new ICollection[0],
                                                    Iri = hydra.BasicRepresentation,
                                                    Links = new ILink[0],
                                                    Operations = new IOperation[0],
                                                    Type = new Iri[0]
                                                }
                                            }
                                        },
                                        Operations = new IOperation[0],
                                        Relation = hydra.search,
                                        SupportedOperations = new IOperation[0],
                                        Target = (IResource)null,
                                        Template = "http://temp.uri/api/events{?searchPhrase}",
                                        Type = new[] { hydra.TemplatedLink }
                                    }
                                },
                                Manages = new IStatement[0],
                                Members = new object[]
                                {
                                    new
                                    {
                                        Iri = new Iri("http://temp.uri/api/events/1"),
                                        Type = new Iri[0]
                                    }
                                },
                                Operations = new IOperation[0],
                                TotalItems = 1,
                                Type = new[] { hydra.Collection },
                                View = new
                                {
                                    First = new
                                    {
                                        Iri = new Iri("http://temp.uri/api/events?page=1"),
                                        Type = new[] { hydra.Link }
                                    },
                                    Iri = new Iri("http://temp.uri/api/events?page=1"),
                                    Last = new
                                    {
                                        Iri = new Iri("http://temp.uri/api/events?page=9"),
                                        Type = new[] { hydra.Link }
                                    },
                                    Type = new[] { hydra.PartialCollectionView }
                                }
                            }
                        },
                        Iri = new Iri("http://temp.uri/api"),
                        Operations = new IOperation[0],
                        Type = new[] { hydra.Resource }
                    }
                },
                _ => _.ExcludingFields().Including(member => RequiredProperties(member)));
        }

        protected override void ScenarioSetup()
        {
            _inputJsonLd = GetResourceNamed("input.json");
            Response.SetupGet(_ => _.Url).Returns(new Uri("http://temp.uri/api", UriKind.Absolute));
            base.ScenarioSetup();
        }
    }
}
