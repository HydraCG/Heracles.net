using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Heracles.DataModel;
using Heracles.Namespaces;
using RDeF.Entities;
using RollerCaster;

namespace Heracles.Rdf.GraphTransformations
{
    /// <summary>Tries to correct missing entry point in hydra:ApiDocumentation resource.</summary>
    public class EntryPointCorrectingGraphTransformer : IGraphTransformer
    {
        private static readonly PropertyInfo EntryPointPropertyInfo =
            typeof(IApiDocumentation).GetProperty(nameof(IApiDocumentation.EntryPoint));
        
        /// <inheritdoc />
        public IEnumerable<ITypedEntity> Transform(
            IEnumerable<ITypedEntity> resources, 
            IHypermediaProcessor processor,
            IHypermediaProcessingOptions options = null)
        {
            var apiDocumentation = resources?.FirstOrDefault(_ => _.Is(hydra.ApiDocumentation))?.ActLike<IApiDocumentation>();
            if (apiDocumentation != null
                && apiDocumentation.EntryPoint == null
                && options != null
                && processor.Supports(options.AuxiliaryResponse) != Level.None
                && Regex.IsMatch(options.AuxiliaryOriginalUrl.ToString(), ".+://[^/]+/?"))
            {
                var entryPoint = apiDocumentation.Context.Create<IResource>(options.AuxiliaryOriginalUrl);
                var proxy = apiDocumentation.Unwrap();
                proxy.SetProperty(EntryPointPropertyInfo, entryPoint);
            }

            return resources;
        }
    }
}