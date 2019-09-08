using System;
using Heracles.DataModel;
using RDeF.Entities;
using RollerCaster;

namespace Heracles.JsonLd
{
    /// <summary>Contains a <see cref="IHydraClient" /> initializer.</summary>
    public partial class JsonLdHypermediaProcessor
    {
        private static IResource PointingResourceInitializer(ITypedEntity resource, IHydraClient client, ProcessingState processingState)
        {
            resource.Unwrap().SetProperty(ResourceExtensions.BaseUrlPropertyInfo, (Uri)processingState.BaseUrl);
            return ResourceInitializer(resource.ActLike<IHydraResource>(), client, processingState);
        }
    }
}