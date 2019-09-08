using System.Collections.Generic;
using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary>Represents a Hydra class.</summary>
    [Class("hydra", "Class")]
    public interface IClass : IHydraResource
    {
        /// <summary>Gets the class' display name.</summary>
        [Property("hydra", "title")]
        string Title { get; }

        /// <summary>Gets the class' description.</summary>
        [Property("hydra", "description")]
        string Description { get; }

        /// <summary>Gets the class' supported operations. </summary>
        [Collection("hydra", "supportedOperation")]
        ISet<IOperation> SupportedOperations { get; }

        /// <summary>Gets the class' supported properties.</summary>
        [Collection("hydra", "supportedProperty")]
        ISet<ISupportedProperty> SupportedProperties { get; }
    }
}