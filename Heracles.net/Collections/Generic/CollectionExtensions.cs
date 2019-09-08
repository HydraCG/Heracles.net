using System.Collections.Generic;
using Heracles.DataModel;
using Heracles.DataModel.Collections;

namespace Heracles.Collections.Generic
{
    internal static class CollectionExtensions
    {
        internal static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> anotherCollection)
        {
            foreach (var item in anotherCollection)
            {
                collection.Add(item);
            }
        }

        internal static void AddIfNotExist<T>(this ICollection<T> collection, T item)
        {
            if (!collection.Contains(item))
            {
                collection.Add(item);
            }
        }

        internal static ISet<ICollection> DiscoverCollections(this IEnumerable<IResource> hypermedia)
        {
            var result = new HashSet<ICollection>();
            foreach (var control in hypermedia)
            {
                if (control.Type.IsCollection())
                {
                    result.Add(control as ICollection);
                }
                else
                {
                    var collections = (control as IHydraResource)?.Collections;
                    if (collections != null)
                    {
                        foreach (var linkedCollection in collections)
                        {
                            result.Add(linkedCollection);
                        }
                    }
                }
            }

            return result;
        }
    }
}
