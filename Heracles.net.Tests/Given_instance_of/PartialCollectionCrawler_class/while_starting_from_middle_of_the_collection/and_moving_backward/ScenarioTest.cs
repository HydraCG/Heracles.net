using System.Collections.Generic;
using System.Threading.Tasks;
using Heracles.DataModel;
using Heracles.Testing;
using RDeF.Entities;

namespace Given_instance_of.PartialCollectionCrawler_class.while_starting_from_middle_of_the_collection.and_moving_backward
{
    public abstract class ScenarioTest : PartialCollectionCrawlerTest
    {
        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            var call = 0;
            var results = new[]
            {
                new[] { Resource.Of<IResource>(new Iri("item:1")).Object },
                new[] { Resource.Of<IResource>(new Iri("item:3")).Object }
            };
            Iterator.Setup(_ => _.GetPreviousPart()).Returns(() => Task.FromResult((IEnumerable<IResource>)results[call++]));
        }
    }
}
