using System.Collections.Generic;
using System.Linq;
using Heracles.Namespaces;
using RDeF.Entities;

namespace Heracles.Entities
{
    public static class EntityExtensions
    {
        private static readonly Iri[] LinkTypes = { hydra.TemplatedLink, hydra.Link };

        private static readonly IDictionary<Iri, Iri> HydraLinks = new Dictionary<Iri, Iri>()
        {
            { hydra.first, hydra.Link },
            { hydra.last, hydra.Link },
            { hydra.previous, hydra.Link },
            { hydra.next, hydra.Link },
            { hydra.view, hydra.Link },
            { hydra.collection, hydra.Link },
            { hydra.search, hydra.TemplatedLink }
        };

        public static Iri GetLinkType(this IEntity predicate)
        {
            Iri result = null;
            if (predicate != null)
            {
                if (!HydraLinks.TryGetValue(predicate.Iri, out result))
                {
                    result = (
                        from type in predicate.GetTypes()
                        join linkType in LinkTypes on type equals linkType
                        select type).FirstOrDefault();
                }
            }

            return result;
        }

        public static Iri GetLinkType(this IEntity entity, LinksPolicy linksPolicy, Iri root)
        {
            Iri result = null;
            if (entity != null)
            {
                result = entity.GetTypes().Any(_ => _ == hydra.IriTemplate) ? hydra.TemplatedLink : null;
                if (result == null && !entity.Iri.IsBlank)
                {
                    if (linksPolicy >= LinksPolicy.SameRoot
                        && root != null
                        && entity.Iri != root
                        && entity.Iri.ToRoot() == root)
                    {
                        result = hydra.Link;
                    }

                    if (result == null && linksPolicy >= LinksPolicy.AllHttp && entity.Iri.IsHttp())
                    {
                        result = hydra.Link;
                    }

                    if (result == null && linksPolicy >= LinksPolicy.All)
                    {
                        result = hydra.Link;
                    }
                }
            }

            return result;
        }
    }
}
