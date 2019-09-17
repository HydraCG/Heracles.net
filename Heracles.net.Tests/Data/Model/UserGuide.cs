using System.Collections.Generic;
using RDeF.Entities;

namespace Heracles.Data.Model
{
    public class UserGuide
    {
        internal IDictionary<Iri, string> Classes { get; } = new Dictionary<Iri, string>();
    }
}
