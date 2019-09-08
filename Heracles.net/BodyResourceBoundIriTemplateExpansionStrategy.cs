using Heracles.DataModel;
using RDeF.Entities;
using RollerCaster;

namespace Heracles
{
    /// <summary>
    /// Provides a simple implementation of the <see cref="IIriTemplateExpansionStrategy" /> interface where an input resource
    /// is used to fill the possible <see cref="IIriTemplate" /> with values.
    /// </summary>
    public class BodyResourceBoundIriTemplateExpansionStrategy : IIriTemplateExpansionStrategy
    {
        /// <inheritdoc />
        public IOperation CreateRequest(
            IOperation operation,
            IResource body = null,
            IResource auxResource = null)
        {
            if (operation is ITemplatedOperation templatedOperation)
            {
                return templatedOperation.ExpandTarget(builder =>
                    WithResourceVariables(builder, body, auxResource)
                );
            }

            return operation;
        }

        private void WithResourceVariables(MappingsBuilder builder, IResource body, IResource auxResource)
        {
            var variableMappings = builder.VariableMappings;
            foreach (var variable in variableMappings)
            {
                var literal = GetValue(body, variable.Value) ?? GetValue(auxResource, variable.Value);
                builder.WithVariable(variable.Key).HavingValueOf(literal);
            }
        }

        private string GetValue(IEntity valueSource, Iri propertyIri)
        {
            string result = null;
            var propertyMapping = valueSource?.Context.Mappings.FindPropertyMappingFor(valueSource, propertyIri);
            if (propertyMapping != null)
            {
                result = valueSource.Unwrap().GetProperty(propertyMapping.PropertyInfo)?.ToString();
            }

            return result;
        }
    }
}