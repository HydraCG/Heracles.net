using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Heracles.Collections.Generic;
using Heracles.JsonLd;
using Heracles.Namespaces;
using RDeF.Entities;
using RollerCaster;
using UriTemplate = Tavis.UriTemplates.UriTemplate;

namespace Heracles.DataModel
{
    /// <summary>Provides a default implementation of the <see cref="ITemplatedResource{T}" /> interface.</summary>
    /// <typeparam name="T">Type of the resource described.</typeparam>
    public static class TemplatedResource<T> where T : IPointingResource
    {
        private static readonly PropertyInfo TargetPropertyInfo = typeof(IPointingResource).GetProperty(nameof(IPointingResource.Target));
        private static readonly PropertyInfo BaseUrlPropertyInfo = typeof(IPointingResource).GetProperty(nameof(IPointingResource.BaseUrl));

        /// <summary>Expands an URI template with given variables.</summary>
        /// <param name="pointingResource">Pointing resource.</param>
        /// <param name="mappedVariables">Template variables with value.</param>
        /// <param name="iri">Iri of the newly created instance of <typeparamref name="T" />.</param>
        /// <returns>Expanded templated resource.</returns>
        public static T ExpandTarget(IPointingResource pointingResource, IDictionary<string, string> mappedVariables, Iri iri)
        {
            var iriTemplate = (IIriTemplate)pointingResource.Unwrap().GetProperty(JsonLdHypermediaProcessor.IriTemplatePropertyInfo);
            var targetUri = new Uri(pointingResource.BaseUrl, new UriTemplate(iriTemplate.Template).AddParameters(mappedVariables).Resolve());
            var result = pointingResource.Context.Create<T>(iri);
            var proxy = result.Unwrap();
            proxy.SetProperty(TargetPropertyInfo, pointingResource.Context.Create<IResource>(targetUri));
            proxy.SetProperty(BaseUrlPropertyInfo, pointingResource.BaseUrl);
            result.Type.AddRange(pointingResource.Type.Where(_ => _ != hydra.IriTemplate));
            result.Collections.AddRange(pointingResource.Collections);
            result.Links.AddRange(pointingResource.Links);
            result.Operations.AddRange(pointingResource.Operations);
            return result;
        }

        /// <summary>Expands an URI template with given variables.</summary>
        /// <param name="pointingResource">Pointing resource.</param>
        /// <param name="mappedVariables">Template variables mapping builder.</param>
        /// <param name="iri">Iri of the newly created instance of <typeparamref name="T" />.</param>
        /// <returns>Expanded templated resource.</returns>
        public static T ExpandTarget(IPointingResource pointingResource, Action<MappingsBuilder> mappedVariables, Iri iri)
        {
            var iriTemplate = (IIriTemplate)pointingResource.Unwrap().GetProperty(JsonLdHypermediaProcessor.IriTemplatePropertyInfo);
            var builder = new MappingsBuilder(iriTemplate.Mappings);
            mappedVariables(builder);
            return ExpandTarget(pointingResource, builder.Complete(), iri);
        }
    }
}