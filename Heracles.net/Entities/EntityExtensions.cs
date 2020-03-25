using System.Collections.Generic;
using System.Linq;
using Heracles.Namespaces;
using RDeF.Entities;
using RollerCaster;

namespace Heracles.Entities
{
    /// <summary>Provides useful <see cref="IEntity" /> extensions.</summary>
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

        /// <summary>Obtains a type of the link of the given <paramref name="predicate" />.</summary>
        /// <param name="predicate">Predicate for which to obtain a link type.</param>
        /// <returns>Determined link type Iri.</returns>
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

        /// <summary>Obtains a type of the link of the given <paramref name="entity" />.</summary>
        /// <param name="entity">Entity for which to obtain a link type.</param>
        /// <param name="linksPolicy">Links policy etermining how far a relation should be considered a link.</param>
        /// <param name="root">Root Iri to be used when resolving some of the policies.</param>
        /// <returns>Determined link type Iri.</returns>
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

        internal static TEntity As<TEntity>(this IEntity entity, Iri type) where TEntity : class, IEntity
        {
            TEntity result;
            if ((result = entity as TEntity) != null
                || (entity.Is(type) && (result = entity.ActLike<TEntity>()) != null))
            {
                return result;
            }

            return null;
        }
    }
}
