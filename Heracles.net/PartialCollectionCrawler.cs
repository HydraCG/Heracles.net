using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Heracles.DataModel;
using RDeF.Entities;

namespace Heracles
{
    /// <summary>Provides capability of crawling through partial collection views.</summary>
    public class PartialCollectionCrawler
    {
        private const int Rewind = 10;
        private const int Forward = 1;
        private const int Backward = -1;
        private const int First = Forward * Rewind;
        private const int Last = Backward * Rewind;

        private static readonly IDictionary<CrawlingDirection, int> KeyMap =
            new Dictionary<CrawlingDirection, int>()
            {
                { CrawlingDirection.Forward, Forward },
                { CrawlingDirection.Backward, Backward }
            };

        private static readonly IDictionary<int, Func<IPartialCollectionIterator, Iri>> DirectedLinks =
            new Dictionary<int, Func<IPartialCollectionIterator, Iri>>()
            {
                { First, _ => _.FirstPartIri },
                { Forward, _ => _.NextPartIri },
                { Backward, _ => _.PreviousPartIri },
                { Last, _ => _.LastPartIri }
            };

        private static readonly IDictionary<int, Func<IPartialCollectionIterator, Func<Task<IEnumerable<IResource>>>>> DirectedGetters =
            new Dictionary<int, Func<IPartialCollectionIterator, Func<Task<IEnumerable<IResource>>>>>()
            {
                { First, _ => _.GetFirstPart },
                { Forward, _ => _.GetNextPart },
                { Backward, _ => _.GetPreviousPart },
                { Last, _ => _.GetLastPart }
            };

        private readonly ICollection _collection;

        private PartialCollectionCrawler(ICollection collection)
        {
            _collection = collection;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PartialCollectionCrawler" />
        /// from a given partial collection view.
        /// </summary>
        /// <param name="collection">Partial collection view to start with.</param>
        /// <returns>Instance of the <see cref="PartialCollectionCrawler" />.</returns>
        public static PartialCollectionCrawler From(ICollection collection)
        {
            return new PartialCollectionCrawler(collection);
        }

        /// <summary>Crawls partial collection views starting with a given one.</summary>
        /// <param name="options">Crawling options.</param>
        /// <returns>Collection of resources from the collection.</returns>
        public async Task<IEnumerable<IResource>> GetMembers(ICrawlingOptions options = null)
        {
            var result = new List<IResource>();
            var memberLimit = options?.MemberLimit ?? Int32.MaxValue;
            var iterator = _collection.GetIterator();
            if (AddWithLimitReached(result, _collection.Members, memberLimit) || iterator == null)
            {
                return result;
            }

            var visitedPages = new List<Iri>() { _collection.View.Iri };
            var requests = 0;
            do
            {
                var direction = options?.Direction ?? CrawlingDirection.Forward;
                var link = DirectedLinks[KeyMap[direction]](iterator);
                var furtherPart = DirectedGetters[KeyMap[direction]](iterator);
                if (link == null && (options?.Rewind ?? false))
                {
                    link = DirectedLinks[KeyMap[direction] * Rewind](iterator);
                    furtherPart = DirectedGetters[KeyMap[direction] * Rewind](iterator);
                }

                if (link == null || visitedPages.Contains(link))
                {
                    break;
                }

                var part = await furtherPart();
                requests++;
                visitedPages.Add(iterator.CurrentPartIri);
                AddWithLimitReached(result, part, memberLimit);
            } while (requests < (options?.RequestLimit ?? Int32.MaxValue) && result.Count < memberLimit);

            return result;
        }

        private bool AddWithLimitReached(ICollection<IResource> result, IEnumerable<IResource> part, int memberLimit)
        {
            foreach (var item in part)
            {
                result.Add(item);
                if (result.Count >= memberLimit)
                {
                    return true;
                }
            }

            return false;
        }
    }
}