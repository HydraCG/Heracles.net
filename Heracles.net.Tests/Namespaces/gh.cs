using System.Diagnostics.CodeAnalysis;
using RDeF.Entities;

namespace Heracles.Namespaces
{
    [SuppressMessage("UnitTests", "TS0000:NoUnitTests", Justification = "No logic is available to be tested.")]
    [ExcludeFromCodeCoverage]
    public class gh
    {
        public static readonly Iri Namespace = new Iri("http://github.com/");

        public static readonly Iri alien_mcl = new Iri((string)Namespace + "alien-mcl");
    }
}