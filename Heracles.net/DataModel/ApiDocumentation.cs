using System.Threading;
using System.Threading.Tasks;
using Heracles.Rdf;
using RollerCaster;

namespace Heracles.DataModel
{
    /// <summary>Provides a default implementation of the <see cref="IApiDocumentation.GetEntryPoint()" /> method.</summary>
    public static class ApiDocumentation
    {
        /// <summary>Retrieves an API's entry point resource.</summary>
        /// <param name="apiDocumentation">API documentation.</param>
        /// <returns>Task with entry point obtained as an instance of the <see cref="IHypermediaContainer" />.</returns>
        public static Task<IHypermediaContainer> GetEntryPoint(IApiDocumentation apiDocumentation)
        {
            return GetEntryPoint(apiDocumentation, CancellationToken.None);
        }

        /// <summary>Retrieves an API's entry point resource.</summary>
        /// <param name="apiDocumentation">API documentation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task with entry point obtained as an instance of the <see cref="IHypermediaContainer" />.</returns>
        public static async Task<IHypermediaContainer> GetEntryPoint(IApiDocumentation apiDocumentation, CancellationToken cancellationToken)
        {
            return await ((IHydraClient)apiDocumentation.Unwrap()
                    .GetProperty(HypermediaProcessorBase.ClientPropertyInfo))
                .GetResource(apiDocumentation.EntryPoint);
        }
    }
}
