using System.Diagnostics.CodeAnalysis;
using RDeF.Entities;

namespace Heracles.Namespaces
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "This reflects an actual namespace prefix. Casing is OK.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Members are reflecting their adequates terms. Casing is OK.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:Static readonly fields should begin with upper-case letter", Justification = "Members are reflecting their adequates terms. Casing is OK.")]
    [SuppressMessage("UnitTests", "TS0000:NoUnitTests", Justification = "No logic is available to be tested.")]
    [ExcludeFromCodeCoverage]
    public class schema
    {
        public static readonly Iri Namespace = new Iri("http://schema.org/");

        public static readonly Iri knows = new Iri((string)Namespace + "knows");
    }
}