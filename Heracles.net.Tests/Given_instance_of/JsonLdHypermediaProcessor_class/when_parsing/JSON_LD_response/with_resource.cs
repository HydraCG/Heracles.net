using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.Namespaces;
using Heracles.Testing;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using RDeF.Entities;
using RollerCaster;
using ICollection = Heracles.DataModel.ICollection;

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
        
        protected override Uri Uri { get; } = new Uri("http://temp.uri/api", UriKind.Absolute);
        
        private Mock<IApiDocumentation> ApiDocumentation { get; set; }
        
        private IOperation Operation { get; set; }

        [Test]
        public void should_transform_resources()
        {
            GraphTransformer.Verify(
                _ => _.Transform(
                    It.IsAny<IEnumerable<ITypedEntity>>(),
                    HypermediaProcessor,
                    HypermediaProcessingOptions.Object),
                Times.Once);
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
                                                    Description = String.Empty,
                                                    DisplayName = String.Empty,
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
                                        Type = new[] { hydra.TemplatedLink, hydra.Resource, rdfs.Resource }
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
                                Operations = new[] { new { Method = "POST" } },
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

        public override void ScenarioSetup()
        {
            _inputJsonLd = GetResourceNamed("input.json");
            Response.SetupGet(_ => _.Url).Returns(Uri);
            base.ScenarioSetup();
            Operation = new MulticastObject().ActLike<IOperation>();
            var entityContext = new Mock<IEntityContext>(MockBehavior.Strict);
            entityContext.Setup(_ => _.Create<IOperation>(It.IsAny<Iri>())).Returns(Operation);
            var proxy = new MulticastObject();
            proxy.SetProperty(typeof(IEntity).GetProperty(nameof(IEntity.Context)), entityContext.Object);
            proxy.SetProperty(typeof(IOperation).GetProperty(nameof(IOperation.Method)), "POST");
            var operation = proxy.ActLike<IOperation>();

            proxy = new MulticastObject();
            proxy.SetProperty(typeof(IEntity).GetProperty(nameof(IEntity.Iri)), hydra.Collection);
            var @class = proxy.ActLike<IClass>();
            @class.SupportedOperations.Add(operation);
            ApiDocumentation = new Mock<IApiDocumentation>(MockBehavior.Strict);
            ApiDocumentation.SetupGet(_ => _.SupportedClasses).Returns(new HashSet<IClass>() { @class });
            HypermediaProcessingOptions.SetupGet(_ => _.ApiDocumentationPolicy).Returns(ApiDocumentationPolicy.FetchAndExtend);
            HypermediaProcessingOptions.SetupGet(_ => _.ApiDocumentations).Returns(new[] { ApiDocumentation.Object });
        }
    }
}
