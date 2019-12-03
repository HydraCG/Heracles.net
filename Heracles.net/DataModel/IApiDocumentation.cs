using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary>Represents an abstract API documentation.</summary>
    [Class("hydra", "ApiDocumentation")]
    public interface IApiDocumentation : IHydraResource
    {
        /// <summary>Gets a title of this API documentation.</summary>
        [Property("hydra", "title")]
        string Title { get; }

        /// <summary>Gets a description of this API documentation.</summary>
        [Property("hydra", "description")]
        string Description { get; }

        /// <summary>Gets the supported classes by this API.</summary>
        [Collection("hydra", "supportedClass")]
        ISet<IClass> SupportedClasses { get; }

        /// <summary>Gets the Url of the entry point of the API.</summary>
        [Property("hydra", "entrypoint")]
        IResource EntryPoint { get; }

        /// <summary>Retrieves an API's entry point resource.</summary>
        /// <returns>Task with entry point obtained as an instance of the <see cref="IHypermediaContainer" />.</returns>
        Task<IHypermediaContainer> GetEntryPoint();

        /// <summary>Retrieves an API's entry point resource.</summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task with entry point obtained as an instance of the <see cref="IHypermediaContainer" />.</returns>
        Task<IHypermediaContainer> GetEntryPoint(CancellationToken cancellationToken);
    }
}
