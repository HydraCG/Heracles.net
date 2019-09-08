using System.Threading.Tasks;
using Heracles.JsonLd;
using RollerCaster;

namespace Heracles.DataModel
{
    /// <summary>Provides a default implementation of the <see cref="IApiDocumentation.GetEntryPoint()" /> method.</summary>
    public static class ApiDocumentation
    {
        /// <summary>Retrieves an API's entry point resource.</summary>
        /// <param name="apiDocumentation">API documentation.</param>
        /// <returns>Task with entry point obtained as an instance of the <see cref="IWebResource" />.</returns>
        public static async Task<IHypermediaContainer> GetEntryPoint(IApiDocumentation apiDocumentation)
        {
            return await ((IHydraClient)apiDocumentation.Unwrap()
                .GetProperty(JsonLdHypermediaProcessor.ClientPropertyInfo))
                .GetResource(apiDocumentation.EntryPoint);
        }
    }
}
