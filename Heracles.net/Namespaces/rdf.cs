using System.Diagnostics.CodeAnalysis;
using RDeF.Entities;

namespace Heracles.Namespaces
{
    /// <summary>Defines rdf namespace and terms.</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "This reflects an actual namespace prefix. Casing is OK.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Members are reflecting their adequates terms. Casing is OK.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:Static readonly fields should begin with upper-case letter", Justification = "Members are reflecting their adequates terms. Casing is OK.")]
    [SuppressMessage("UnitTests", "TS0000:NoUnitTests", Justification = "No logic is available to be tested.")]
    [ExcludeFromCodeCoverage]
    public class rdf
    {
        /// <summary>Defines a rdf namespace IRI.</summary>
        public static readonly Iri Namespace = new Iri("http://www.w3.org/1999/02/22-rdf-syntax-ns#");

        /// <summary>Defines a rdf:Property predicate IRI.</summary>
        public static readonly Iri Property = new Iri((string)Namespace + "Property");
 
        /// <summary>Defines a rdf:type predicate IRI.</summary>
        public static readonly Iri type = new Iri((string)Namespace + "type");
    }
}