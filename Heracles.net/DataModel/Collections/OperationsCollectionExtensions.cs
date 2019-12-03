using System;
using System.Collections.Generic;
using System.Linq;
using Heracles.Namespaces;
using RDeF.Entities;

namespace Heracles.DataModel.Collections
{
    /// <summary>
    /// Provides extensions methods of <see cref="IEnumerable{IOperation}" />
    /// that can be filtered with relevant criteria.
    /// </summary>
    public static class OperationsCollectionExtensions
    {
        /// <summary>Obtains a collection of operations using a given method.</summary>
        /// <param name="operations">Collection to be filtered.</param>
        /// <param name="method">Method to match.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<IOperation> OfMethod(this IEnumerable<IOperation> operations, string method)
        {
            var result = operations;
            if (operations != null && !String.IsNullOrEmpty(method))
            {
                result = result.Where(item => item.Method == method);
            }

            return result;
        }

        /// <summary>Obtains a collection of operations expecting a given type.</summary>
        /// <param name="operations">Collection to be filtered.</param>
        /// <param name="iri">Expected type.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<IOperation> Expecting(this IEnumerable<IOperation> operations, Iri iri)
        {
            var result = operations;
            if (operations != null && iri != null)
            {
                result = result.Where(item => item.Expects.Any(_ => _.Iri == iri));
            }

            return result;
        }
        
        /// <summary>Obtains a collection of operations returning a given type.</summary>
        /// <param name="operations">Collection to be filtered.</param>
        /// <param name="iri">Returned type.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<IOperation> Returning(this IEnumerable<IOperation> operations, Iri iri)
        {
            var result = operations;
            if (operations != null && iri != null)
            {
                result = result.Where(item => item.Returns.Any(_ => _.Iri == iri));
            }

            return result;
        }

        /// <summary>Obtains a collection of operations returning a given type.</summary>
        /// <param name="operations">Collection to be filtered.</param>
        /// <param name="name">Expected header name.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<IOperation> ExpectingHeader(this IEnumerable<IOperation> operations, string name)
        {
            var result = operations;
            if (operations != null && !String.IsNullOrEmpty(name))
            {
                result = result.Where(item => item.ExpectedHeaders.Contains(name));
            }

            return result;
        }

        /// <summary>Obtains a collection of operations returning a header.</summary>
        /// <param name="operations">Collection to be filtered.</param>
        /// <param name="name">Returned header name.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<IOperation> ReturningHeader(this IEnumerable<IOperation> operations, string name)
        {
            var result = operations;
            if (operations != null && !String.IsNullOrEmpty(name))
            {
                result = result.Where(item => item.ReturnedHeaders.Contains(name));
            }

            return result;
        }

        /// <summary>Obtains a collection of operations being an hydra:IriTemplate.</summary>
        /// <param name="operations">Operation to iterate through.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<IOperation> WithTemplate(this IEnumerable<IOperation> operations)
        {
            return operations.OfType(hydra.IriTemplate);
        }
    }
}
