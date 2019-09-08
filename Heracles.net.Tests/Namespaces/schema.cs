using System.Diagnostics.CodeAnalysis;
using RDeF.Entities;

namespace Heracles.Namespaces
{
    [SuppressMessage("UnitTests", "TS0000:NoUnitTests", Justification = "No logic is available to be tested.")]
    [ExcludeFromCodeCoverage]
    public class schema
    {
        public static readonly Iri Namespace = new Iri("http://schema.org/");

        public static readonly Iri knows = new Iri((string)Namespace + "knows");
    }
}