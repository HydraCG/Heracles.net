using Heracles.Rdf;
using RollerCaster;

namespace Heracles.DataModel
{
    /// <summary>Provides a default implementation of the <see cref="ICollection.GetIterator()" /> interface.</summary>
    public static class Collection
    {
        /// <summary>Gets a partial collection iterator associated in case it is a partial one.</summary>
        /// <returns>Instance of the <see cref="IPartialCollectionIterator" />.</returns>
        public static IPartialCollectionIterator GetIterator(ICollection collection)
        {
            return collection.View == null
                ? null
                : new PartialCollectionIterator(
                    collection,
                    (IHydraClient)collection.Unwrap().GetProperty(HypermediaProcessorBase.ClientPropertyInfo));
        }
    }
}