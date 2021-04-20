using System;
using System.Collections.Generic;
using Heracles.DataModel;

namespace Heracles
{
    /// <summary>Describes a <see cref="IHypermediaProcessor"/> processing options.</summary>
    public interface IHypermediaProcessingOptions
    {
        /// <summary>Gets a policy defining which related resources will be added to links collection.</summary>
        LinksPolicy LinksPolicy { get; }
        
        /// <summary>Gets a policy defining how API documentations should be handled when obtaining resources.</summary>
        ApiDocumentationPolicy ApiDocumentationPolicy { get; }
        
        /// <summary>Gets an API documentations obtained.</summary>
        IEnumerable<IApiDocumentation> ApiDocumentations { get; }

        /// <summary>
        /// Gets an originally requested Url.
        /// This may be different than the one provided in the Response.url after redirects.
        /// </summary>
        Uri OriginalUrl { get; }

        /// <summary>Gets an auxiliary response that was used to obtain currently processed one.</summary>
        IResponse AuxiliaryResponse { get; }

        /// <summary>Gets an original auxiliary Url requested that was used to obtain currently processed one.</summary>
        /// <remarks>This property should be set in case {@link auxiliaryResponse} is also set.</remarks>
        Uri AuxiliaryOriginalUrl { get; }
    }
}