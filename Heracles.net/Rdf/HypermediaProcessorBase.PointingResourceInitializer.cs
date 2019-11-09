using System;
using System.Threading;
using System.Threading.Tasks;
using Heracles.DataModel;
using RDeF.Entities;
using RollerCaster;

namespace Heracles.Rdf
{
    /// <summary>Contains a <see cref="IHydraClient" /> initializer.</summary>
    public partial class HypermediaProcessorBase
    {
        private static Task<IResource> PointingResourceInitializer(
            ITypedEntity resource,
            IHydraClient client,
            ProcessingState processingState,
            CancellationToken cancellationToken)
        {
            resource.Unwrap().SetProperty(ResourceExtensions.BaseUrlPropertyInfo, (Uri)processingState.BaseUrl);
            return ResourceInitializer(resource.ActLike<IHydraResource>(), client, processingState, cancellationToken);
        }
    }
}