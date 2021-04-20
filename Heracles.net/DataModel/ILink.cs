using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary>Describes a link to another resource.</summary>
    [Class("hydra", "Link")]
    public interface ILink : IDereferencableLink
    {
        /// <summary>Gets the link's title.</summary>
        [Property("hydra", "title", DefaultValue = "")]
        string Title { get; }

        /// <summary>Gets the link's description.</summary>
        [Property("hydra", "description", DefaultValue = "")]
        string Description { get; }
    }
}