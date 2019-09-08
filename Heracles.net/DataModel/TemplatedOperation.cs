using System;
using System.Collections.Generic;
using System.Reflection;
using Heracles.Collections.Generic;
using RDeF.Entities;
using RollerCaster;

namespace Heracles.DataModel
{
    /// <summary>Provides a default implementation of the <see cref="ITemplatedOperation"/> interface.</summary>
    public static class TemplatedOperation
    {
        private static readonly PropertyInfo MethodPropertyInfo = typeof(IOperation).GetProperty(nameof(IOperation.Method));

        private static int _id;

        /// <summary>Expands an URI template with given variables.</summary>
        /// <param name="templatedOperation">Templated operation.</param>
        /// <param name="mappedVariables">Template variables with value.</param>
        /// <returns>Expanded templated resource.</returns>
        public static IOperation ExpandTarget(ITemplatedOperation templatedOperation, IDictionary<string, string> mappedVariables)
        {
            return TemplatedResource<IOperation>.ExpandTarget(templatedOperation, mappedVariables, GetNextIri())
                .CreateInstanceFrom(templatedOperation);
        }

        /// <summary>Expands an URI template with given variables.</summary>
        /// <param name="templatedOperation">Templated operation.</param>
        /// <param name="mappedVariables">Template variables mapping builder.</param>
        /// <returns>Expanded templated resource.</returns>
        public static IOperation ExpandTarget(ITemplatedOperation templatedOperation, Action<MappingsBuilder> mappedVariables)
        {
            return TemplatedResource<IOperation>.ExpandTarget(templatedOperation, mappedVariables, GetNextIri())
                .CreateInstanceFrom(templatedOperation);
        }

        private static IOperation CreateInstanceFrom(this IOperation result, ITemplatedOperation templatedOperation)
        {
            var proxy = result.Unwrap();
            proxy.SetProperty(MethodPropertyInfo, templatedOperation.Method);
            result.Expects.AddRange(templatedOperation.Expects);
            result.Returns.AddRange(templatedOperation.Returns);
            result.ExpectedHeaders.AddRange(templatedOperation.ExpectedHeaders);
            result.ReturnedHeaders.AddRange(templatedOperation.ReturnedHeaders);
            return result;
        }

        private static Iri GetNextIri()
        {
            return new Iri($"_:blankOperation{++_id}");
        }
    }
}