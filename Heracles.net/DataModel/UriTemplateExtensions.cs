using System.Collections.Generic;
using Tavis.UriTemplates;

namespace Heracles.DataModel
{
    internal static class UriTemplateExtensions
    {
        internal static UriTemplate AddParameters(this UriTemplate uriTemplate, IDictionary<string, string> mappedVariables)
        {
            foreach (var variable in mappedVariables)
            {
                uriTemplate.SetParameter(variable.Key, variable.Value);
            }

            return uriTemplate;
        }
    }
}
