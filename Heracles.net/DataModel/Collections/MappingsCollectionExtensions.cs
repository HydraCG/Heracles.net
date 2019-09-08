using System;
using System.Collections.Generic;
using System.Linq;
using RDeF.Entities;

namespace Heracles.DataModel.Collections
{
    /// <summary>
    /// Provides extensions methods of <see cref="IEnumerable{IIriTemplateMapping}" />
    /// that can be filtered with relevant criteria.
    /// </summary>
    public static class MappingsCollectionExtensions
    {
        /// <summary>Obtains a collection of mappings for a given variable name.</summary>
        /// <param name="mappings">Collection to be filtered.</param>
        /// <param name="variableName">Variable name.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<IIriTemplateMapping> OfVariableName(
            this IEnumerable<IIriTemplateMapping> mappings,
            string variableName)
        {
            var result = mappings;
            if (result != null && !String.IsNullOrEmpty(variableName))
            {
                result = result.Where(item => item.Variable == variableName);
            }

            return result;
        }
        
        /// <summary>Obtains a collection of mappings for a given predicate.</summary>
        /// <param name="mappings">Collection to be filtered.</param>
        /// <param name="property">Predicate IRI.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<IIriTemplateMapping> OfProperty(
            this IEnumerable<IIriTemplateMapping> mappings,
            Iri property)
        {
            var result = mappings;
            if (mappings != null && property != null)
            {
                result = result.Where(item => item.Property.Iri == property);
            }

            return result;
        }
    }
}
