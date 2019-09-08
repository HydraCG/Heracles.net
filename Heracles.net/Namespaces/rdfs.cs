using System.Diagnostics.CodeAnalysis;
using RDeF.Entities;

namespace Heracles.Namespaces
{
    /// <summary>Defines rdfs namespace and terms.</summary>
    [SuppressMessage("UnitTests", "TS0000:NoUnitTests", Justification = "No logic is available to be tested.")]
    [ExcludeFromCodeCoverage]
    public class rdfs
    {
        /// <summary>Defines a rdfs namespace IRI.</summary>
        public static readonly Iri Namespace = new Iri("http://www.w3.org/2000/01/rdf-schema#");

        /// <summary>Defines a rdfs:Resource predicate IRI.</summary>
        public static readonly Iri Resource = new Iri((string)Namespace + "Resource");

        /// <summary>Defines a rdfs:comment predicate IRI.</summary>
        public static readonly Iri comment = new Iri((string)Namespace + "comment");
 
        /// <summary>Defines a rdfs:domain predicate IRI.</summary>
        public static readonly Iri domain = new Iri((string)Namespace + "domain");
 
        /// <summary>Defines a rdfs:label predicate IRI.</summary>
        public static readonly Iri label = new Iri((string)Namespace + "label");
 
        /// <summary>Defines a rdfs:range predicate IRI.</summary>
        public static readonly Iri range = new Iri((string)Namespace + "range");
    }
}