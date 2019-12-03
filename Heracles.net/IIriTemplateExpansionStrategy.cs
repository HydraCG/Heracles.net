using Heracles.DataModel;

namespace Heracles
{
    /// <summary>
    /// Describes a strategy of <see cref="DataModel.IOperation" /> and <see cref="IIriTemplate" /> expansion.
    /// </summary>
    public interface IIriTemplateExpansionStrategy
    {
        /// <summary>
        /// Creates a fully invocable <see cref="DataModel.IOperation" /> taking into account possible
        /// IRI template used by the input operation.
        /// </summary>
        /// <param name="operation">Source operation describing the request.</param>
        /// <param name="body">Optional resource to be placed in the body of the request.</param>
        /// <param name="auxResource">Optional auxiliar resource to be used for variable mappings.</param>
        /// <returns>Operation that can be invoked.</returns>
        IOperation CreateRequest(IOperation operation, IResource body = null, IResource auxResource = null);
    }
}