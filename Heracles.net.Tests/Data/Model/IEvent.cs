using Heracles.DataModel;
using RDeF.Mapping.Attributes;

namespace Heracles.Data.Model
{
    public interface IEvent : IResource
    {
        [Property(Iri = "http://schema.org/description")]
        string Description { get; set; }

        [Property(Iri = "http://schema.org/name")]
        string Name { get; set; }
    }
}
