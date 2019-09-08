using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary>Describes an abstract Hydra IRI template mapping.</summary>
    [Class("hydra", "IriTemplateMapping")]
    public interface IIriTemplateMapping : IResource
    {
        /// <summary>Gets a variable name being mapped.</summary>
        [Property("hydra", "variable")]
        string Variable { get; set; }

        /// <summary>Gets a property used for this variable mapping.</summary>
        [Property("hydra", "property")]
        IResource Property { get; set; }

        /// <summary>Gets the value indicating whether the mapping is required or no.</summary>
        [Property("hydra", "required")]
        bool Required { get; set; }
    }
}