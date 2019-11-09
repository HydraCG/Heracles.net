using Heracles.Rdf;

namespace Heracles
{
    /// <summary>Provides an abstraction over facility used for creating Hydra clients.</summary>
    public interface IHydraClientFactory
    {
        /// <summary>Gets a currently configured facility for HTTP communication.</summary>
        HttpCallFacility CurrentHttpCall { get; }

        /// <summary>Gets a currently configured <see cref="LinksPolicy" />.</summary>
        LinksPolicy CurrentLinksPolicy { get; }

        /// <summary>Gets an ontology provider.</summary>
        IOntologyProvider OntologyProvider { get; }
    }
}
