﻿using Heracles.DataModel;
using Heracles.Namespaces;
using RDeF.Entities;
using RollerCaster;

namespace Heracles.JsonLd
{
    /// <summary>Contains a <see cref="IHydraClient" /> initializer.</summary>
    public partial class JsonLdHypermediaProcessor
    {
        private static IResource ClientInitializer(ITypedEntity resource, IHydraClient client, ProcessingState processingState)
        {
            resource.Unwrap().SetProperty(ClientPropertyInfo, client);
            return ResourceInitializer(
                resource.Is(hydra.ApiDocumentation) ? (ITypedEntity)resource.ActLike<IApiDocumentation>() : resource.ActLike<ICollection>(),
                client,
                processingState);
        }
    }
}