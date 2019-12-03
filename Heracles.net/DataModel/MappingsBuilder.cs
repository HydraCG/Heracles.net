using System.Collections.Generic;
using System.Linq;
using Heracles.DataModel.Collections;
using RDeF.Entities;

namespace Heracles.DataModel
{
    /// <summary>Provides a builder for <see cref="IIriTemplate" /> variable mapping values.</summary>
    public class MappingsBuilder
    {
        private readonly IEnumerable<IIriTemplateMapping> _mappings;
        private readonly IDictionary<string, string> _result;

        /// <summary>Initializes a new instance of the <see cref="MappingsBuilder" /> class.</summary>
        /// <param name="mappings">IRI template variable mappings collection.</param>
        public MappingsBuilder(IEnumerable<IIriTemplateMapping> mappings)
        {
            _mappings = mappings;
            _result = new Dictionary<string, string>();
        }

        /// <summary>Gets variable mappings in form of variable name - property IRI pairs.</summary>
        public IDictionary<string, Iri> VariableMappings
        {
            get
            {
                var result = new Dictionary<string, Iri>();
                foreach (var mapping in _mappings)
                {
                    result[mapping.Variable] = mapping.Property.Iri;
                }

                return result;
            }
        }

        /// <summary>Allows to add an IRI property value.</summary>
        /// <param name="property">IRI of the property to be filled with value.</param>
        /// <returns>Property mapping.</returns>
        public PropertyMapping WithProperty(Iri property)
        {
            var mapping = _mappings.OfProperty(property).First();
            return new PropertyMapping(this, _result, mapping);
        }

        /// <summary>Allows to add a direct variable value.</summary>
        /// <param name="variableName">IRI of the property to be filled with value.</param>
        /// <returns>Property mapping.</returns>
        public PropertyMapping WithVariable(string variableName)
        {
            var mapping = _mappings.OfVariableName(variableName).First();
            return new PropertyMapping(this, _result, mapping);
        }

        /// <summary>Completes the variable values mappings in form of variable name - serialized value pairs.</summary>
        /// <returns>Variable mappings.</returns>
        public IDictionary<string, string> Complete()
        {
            return _result;
        }
    }
}