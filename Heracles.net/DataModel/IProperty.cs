using System.Collections.Generic;
using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary> Describes an abstract property.</summary>
    [Class("hydra", "Property")]
    public interface IProperty : IResource
    {
        /// <summary>Gets the property' title.</summary>
        [Property("hydra", "title", DefaultValue = "")]
        string Title { get; }

        /// <summary>Gets the property' description.</summary>
        [Property("hydra", "description", DefaultValue = "")]
        string Description { get; }
        
        /// <summary>Gets the types of values this property can have.</summary>
        [Property("rdfs", "range")]
        ISet<IResource> ValuesOfType { get; }
    }
}