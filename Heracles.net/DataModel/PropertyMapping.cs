using System.Collections.Generic;

namespace Heracles.DataModel
{
    /// <summary>Provides a property mapping bulding facility.</summary>
    public class PropertyMapping
    {
        private readonly MappingsBuilder _builder;
        private readonly IDictionary<string, string> _result;
        private readonly IIriTemplateMapping _mapping;

        /// <summary>Initializes a new instance of the <see cref="PropertyMapping" /> class.</summary>
        /// <param name="builder">Mappings builder.</param>
        /// <param name="result">Current property mappings.</param>
        /// <param name="mapping">IRI template mapping.</param>
        internal PropertyMapping(
            MappingsBuilder builder,
            IDictionary<string, string> result,
            IIriTemplateMapping mapping)
        {
            _builder = builder;
            _result = result;
            _mapping = mapping;
        }

        /// <summary>Allows to map a value to a variable mapping.</summary>
        /// <param name="value">Value to be used.</param>
        /// <returns>Mappings builder.</returns>
        public MappingsBuilder HavingValueOf(string value)
        {
            _result[_mapping.Variable] = value;
            return _builder;
        }
    }
}