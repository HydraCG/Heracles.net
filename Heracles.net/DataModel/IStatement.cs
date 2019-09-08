using RDeF.Entities;
using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary>Describes an abstract statement.</summary>
    /// <remarks>
    /// While all properties are optional, it is recommended to provide at least two of the properties.
    /// </remarks>
    public interface IStatement : IEntity
    {
        /// <summary>Gets the IRI of the statement's subject.</summary>
        [Property("hydra", "subject")]
        IResource Subject { get; }

        /// <summary>Gets the IRI of the statement's predicate.</summary>
        [Property("hydra", "property")]
        IResource Property { get; }

        /// <summary>Gets the IRI of the statement's object.</summary>
        [Property("hydra", "object")]
        IResource Object { get; }
    }
}