namespace Heracles
{
    /// <summary>Defines API documentation discovery policies.</summary>
    public enum ApiDocumentationPolicy
    {
        /// <summary>
        /// Defines that no explicit calls for the API documentation should be invoked.
        /// </summary>
        None,

        /// <summary>
        /// Defines that API documentation should be fetched only, leaving responses with original hypermedia controls.
        /// </summary>
        FetchOnly,
        
        /// <summary>
        /// Defines that API documentation should be fetched and responses should be extended with additional details.
        /// </summary>
        FetchAndExtend
    }
}