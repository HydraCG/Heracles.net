using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FluentAssertions.Equivalency;
using Heracles.DataModel;
using Heracles.Namespaces;
using Moq;
using RDeF.Entities;

namespace Given_instance_of.JsonLdHypermediaProcessor_class.when_parsing.JSON_LD_response
{
    public abstract class ScenarioTest : JsonLdHypermediaProcessorTest
    {
        private static readonly string[] AllowedProperties =
        {
            nameof(IHypermediaContainer.Collections),
            nameof(IHypermediaContainer.Links),
            nameof(IHypermediaContainer.Type),
            nameof(IHypermediaContainer.Members),
            nameof(IHypermediaContainer.View)
        };

        private static readonly IDictionary<Iri, Iri> Domains = new Dictionary<Iri, Iri>()
        {
            { hydra.view, hydra.Collection },
            { hydra.collection, hydra.Resource }
        };

        private static readonly IDictionary<Iri, Iri> Ranges = new Dictionary<Iri, Iri>()
        {
            { hydra.collection, hydra.Collection }
        };

        protected abstract Stream InputJsonLd { get; }

        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            ResponseHeaders.SetupGet(_ => _["Content-Type"]).Returns(new[] { "application/ld+json" });
            Response.Setup(_ => _.GetBody(It.IsAny<CancellationToken>())).ReturnsAsync(InputJsonLd);
            OntologyProvider.Setup(_ => _.GetDomainFor(It.IsAny<Iri>()))
                .Returns<Iri>(predicate => Domains.ContainsKey(predicate) ? Domains[predicate] : null);
            OntologyProvider.Setup(_ => _.GetRangeFor(It.IsAny<Iri>()))
                .Returns<Iri>(predicate => Ranges.ContainsKey(predicate) ? Ranges[predicate] : null);
        }

        protected static bool RequiredProperties(IMemberInfo memberInfo)
        {
            return AllowedProperties.Contains(memberInfo.SelectedMemberInfo.Name);
        }

        protected static class foaf
        {
            internal static readonly Iri homePage = new Iri("http://xmlns.com/foaf/0.1/homepage");
        }

        protected static class schema
        {
            internal static readonly Iri Person = new Iri("http://schema.org/Person");

            internal static readonly Iri knows = new Iri("http://schema.org/knows");
        }

        protected static class Api
        {
            internal static class People
            {
                internal static readonly Iri Karol = new Iri("http://temp.uri/api/people/karol");

                internal static readonly Iri Markus = new Iri("http://temp.uri/api/people/markus");
            }
        }
    }
}
