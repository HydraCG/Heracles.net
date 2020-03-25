using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary>Describes an abstract Hydra property.</summary>
    [Class("hydra", "SupportedProperty")]
    public interface ISupportedProperty : IHydraResource
    {
        /// <summary>Gets the supported property's title.</summary>
        [Property("hydra", "title", DefaultValue = "")]
        string Title { get; }

        /// <summary>Gets the supported property's description.</summary>
        [Property("hydra", "description", DefaultValue = "")]
        string Description { get; }

        /// <summary>Gets the actual property.</summary>
        [Property("hydra", "property")]
        IProperty Property { get; }

        /// <summary>Gets a value indicating whether this property is required.</summary>
        [Property("hydra", "required")]
        bool Required { get; }

        /// <summary>Gets a value indicating whether this property is readable.</summary>
        [Property("hydra", "readable")]
        bool Readable { get; }

        /// <summary>Gets a value indicating whether this property is writable.</summary>
        [Property("hydra", "writeable")]
        bool Writable { get; }
    }
}