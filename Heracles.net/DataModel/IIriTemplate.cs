using System.Collections.Generic;
using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary>Describes an abstract Hydra IRI template.</summary>
    [Class("hydra", "IriTemplate")]
    public interface IIriTemplate : IHydraResource
    {
        /// <summary>Gets an URI template.</summary>
        [Property("hydra", "template")]
        string Template { get; }

        /// <summary>Gets a variable representation type.</summary>
        [Property("hydra", "validRepresentation")]
        IResource VariableRepresentation { get; }

        /// <summary>Gets the variable mappings.</summary>
        [Collection("hydra", "mapping")]
        ISet<IIriTemplateMapping> Mappings { get; }
    }
}
