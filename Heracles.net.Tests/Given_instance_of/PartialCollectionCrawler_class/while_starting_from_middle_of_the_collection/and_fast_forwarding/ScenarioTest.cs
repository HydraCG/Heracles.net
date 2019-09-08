using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Heracles.DataModel;
using Heracles.Testing;
using RDeF.Entities;

namespace Given_instance_of.PartialCollectionCrawler_class.while_starting_from_middle_of_the_collection.and_fast_forwarding
{
    public abstract class ScenarioTest : PartialCollectionCrawlerTest
    {
        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            var call = 0;
            var results = new Func<IEnumerable<IResource>>[]
            {
                () => new[] { Resource.Of<IResource>(new Iri("item:3")).Object },
                () => new[] { Resource.Of<IResource>(new Iri("item:4")).Object }
            };
            Iterator.Setup(_ => _.GetNextPart()).Returns(() => Task.FromResult(results[call++]()));
        }
    }
}
