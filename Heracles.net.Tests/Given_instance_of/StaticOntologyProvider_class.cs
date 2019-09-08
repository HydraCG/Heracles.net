using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Heracles.JsonLd;
using Heracles.Namespaces;
using JsonLD.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RDeF.Entities;

namespace Given_instance_of
{
    [TestFixture]
    public class StaticOntologyProvider_class
    {
        private static readonly string ResourceName = typeof(StaticOntologyProvider).Assembly.GetManifestResourceNames().First(_ => _.EndsWith("hydra.json"));

        private static readonly IDictionary<Iri, Iri> PropertyDomain = new Dictionary<Iri, Iri>()
        {
            { hydra.entrypoint, hydra.ApiDocumentation },
            { hydra.supportedClass, hydra.ApiDocumentation },
            { hydra.supportedProperty, hydra.Class },
            { hydra.readable, hydra.SupportedProperty },
            { hydra.writeable, hydra.SupportedProperty },
            { hydra.operation, hydra.Resource },
            { hydra.method, hydra.Operation },
            { hydra.expects, hydra.Operation },
            { hydra.returns, hydra.Operation },
            { hydra.member, hydra.Collection },
            { hydra.view, hydra.Resource },
            { hydra.totalItems, hydra.Collection },
            { hydra.first, hydra.Resource },
            { hydra.last, hydra.Resource },
            { hydra.next, hydra.Resource },
            { hydra.previous, hydra.Resource },
            { hydra.mapping, hydra.IriTemplate },
            { hydra.variable, hydra.IriTemplateMapping },
        };

        private static readonly IDictionary<Iri, Iri> PropertyRange = new Dictionary<Iri, Iri>()
        {
            { hydra.property, rdf.Property },
            { hydra.apiDocumentation, hydra.ApiDocumentation },
            { hydra.entrypoint, hydra.Resource },
            { hydra.supportedClass, hydra.Class },
            { hydra.possibleStatus, hydra.Status },
            { hydra.supportedProperty, hydra.SupportedProperty },
            { hydra.supportedOperation, hydra.Operation },
            { hydra.operation, hydra.Operation },
            { hydra.expects, hydra.Class },
            { hydra.returns, hydra.Class },
            { hydra.member, hydra.Resource },
            { hydra.view, hydra.Resource },
            { hydra.first, hydra.Resource },
            { hydra.next, hydra.Resource },
            { hydra.previous, hydra.Resource },
            { hydra.search, hydra.IriTemplate },
            { hydra.variableRepresentation, hydra.VariableRepresentation },
            { hydra.mapping, hydra.IriTemplateMapping },
        };

        private static readonly JToken Hydra = (JToken)new JsonSerializer().Deserialize(
            new JsonTextReader(new StreamReader(typeof(StaticOntologyProvider).Assembly.GetManifestResourceStream(ResourceName))));

        private static readonly StaticOntologyProvider Provider = new StaticOntologyProvider(
            new JsonLdApi(new JsonLdApi().Expand(new Context(), Hydra), new JsonLdOptions(String.Empty))
                    .ToRDF().AsStatements());

        [Test]
        public void should_get_a_correct_domain()
        {
            foreach (var predicate in PropertyDomain)
            {
                Provider.GetDomainFor(predicate.Key).Should().Be(predicate.Value);
            }
        }
        
        [Test]
        public void should_get_a_correct_range()
        {
            foreach (var predicate in PropertyRange)
            {
                Provider.GetRangeFor(predicate.Key).Should().Be(predicate.Value);
            }
        }
    }
}
