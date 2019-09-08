namespace Heracles
{
    /// <summary>Defines possible links policies.</summary>
    public enum LinksPolicy
    {
        /// <summary>
        /// Defines that only predicates that are marked with hydra:Link or hydra:TemplatedLink are considered a link.
        /// </summary>
        Strict,

        /// <summary>
        /// Defines that all resources in a relation pointing to the same protocol, host and port are considered a link.
        /// </summary>
        SameRoot,

        /// <summary>Defines that all non-blank HTTP/HTTPS resources in a relation are considered a link.</summary>
        AllHttp,

        /// <summary>Defines that all non-blank resources in a relation are considered a link.</summary>
        All
    }
}