using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary>Provides a link that can has an URI template.</summary>
    [Class("hydra", "TemplatedLink")]
    public interface ITemplatedLink : IDerefencableLink, ITemplatedResource<ILink>
    {
    }
}