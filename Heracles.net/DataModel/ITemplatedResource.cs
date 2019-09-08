using System;
using System.Collections.Generic;

namespace Heracles.DataModel
{
    /// <summary>Provides an abstract description of a resource with expandable template.</summary>
    /// <typeparam name="T">Type of the resource with template.</typeparam>
    public interface ITemplatedResource<T> : IPointingResource where T : IPointingResource
    {
        /// <summary>Expands an URI template with given variables.</summary>
        /// <param name="mappedVariables">Template variables with value.</param>
        /// <returns>Expanded templated resource.</returns>
        T ExpandTarget(IDictionary<string, string> mappedVariables);
        
        /// <summary>Expands an URI template with given variables.</summary>
        /// <param name="mappedVariables">Template variables mapping builder.</param>
        /// <returns>Expanded templated resource.</returns>
        T ExpandTarget(Action<MappingsBuilder> mappedVariables);
    }
}