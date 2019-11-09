using System.Collections.Generic;
using Heracles.Namespaces;
using RDeF.Entities;

namespace Heracles.Rdf
{
    /// <summary>Provides a simple implementation of the RDF predicate range-domain provider that uses statically provided ontology.</summary>
    public class StaticOntologyProvider : IOntologyProvider
    {
        private readonly IDictionary<Iri, IDictionary<Iri, Iri>> _ontology;

        /// <summary>Initializes a new instance of the <see cref="StaticOntologyProvider" /> class.</summary>
        /// <param name="rdfSet">RDF data set.</param>
        public StaticOntologyProvider(IEnumerable<Statement> rdfSet)
        {
            _ontology = new Dictionary<Iri, IDictionary<Iri, Iri>>();
            if (rdfSet != null)
            {
                _ontology = EnsureInitialized(rdfSet);
            }
        }

        /// <inheritdoc />
        public Iri GetDomainFor(Iri predicate)
        {
            return GetValueOf(predicate, rdfs.domain);
        }

        /// <inheritdoc />
        public Iri GetRangeFor(Iri predicate)
        {
            return GetValueOf(predicate, rdfs.range);
        }

        private IDictionary<Iri, IDictionary<Iri, Iri>> EnsureInitialized(IEnumerable<Statement> rdfSet)
        {
            var map = new Dictionary<Iri, IDictionary<Iri, Iri>>();
            foreach (var statement in rdfSet)
            {
                if (!map.ContainsKey(statement.Subject))
                {
                    map[statement.Subject] = new Dictionary<Iri, Iri>();
                }

                if (!map[statement.Subject].ContainsKey(statement.Predicate) && statement.Object != null)
                {
                    map[statement.Subject][statement.Predicate] = statement.Object;
                }
            }

            return map;
        }

        private Iri GetValueOf(Iri iri, Iri predicate)
        {
            return _ontology.TryGetValue(iri, out IDictionary<Iri, Iri> term) && term.TryGetValue(predicate, out Iri result) ? result : null;
        }
    }
}
