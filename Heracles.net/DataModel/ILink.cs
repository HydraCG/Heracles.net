using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary>Describes a link to another resource.</summary>
    [Class("hydra", "Link")]
    public interface ILink : IDereferencableLink
    {
    }
}