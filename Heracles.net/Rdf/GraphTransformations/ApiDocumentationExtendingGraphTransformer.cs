using System.Collections.Generic;
using System.Linq;
using Heracles.DataModel;
using RDeF.Entities;
using RollerCaster;

namespace Heracles.Rdf.GraphTransformations
{
    /// <summary>Extends resources with <see cref="IApiDocumentation" />'s discovered capabilities.</summary>
    public class ApiDocumentationExtendingGraphTransformer : IGraphTransformer
    {
        /// <inheritdoc />
        public IEnumerable<ITypedEntity> Transform(
            IEnumerable<ITypedEntity> resources,
            IHypermediaProcessor processor,
            IHypermediaProcessingOptions options = null)
        {
            if (options != null && options.ApiDocumentations.Any())
            {
                foreach (var resource in resources)
                {
                    var validSupportedClasses =
                        from apiDocumentation in options.ApiDocumentations
                        from supportedClass in apiDocumentation.SupportedClasses
                        where resource.Is(supportedClass.Iri)
                        select supportedClass;
                    IHydraResource dereferencableResource = null;
                    foreach (var supportedClass in validSupportedClasses)
                    {
                        dereferencableResource = dereferencableResource ?? resource.AssertAs<IHydraResource>();
                        foreach (var operation in supportedClass.SupportedOperations)
                        {
                            dereferencableResource.Operations.Add(
                                operation.Copy(new Iri(), options.OriginalUrl, dereferencableResource));
                        }
                    }
                }
            }

            return resources;
        }
    }
}