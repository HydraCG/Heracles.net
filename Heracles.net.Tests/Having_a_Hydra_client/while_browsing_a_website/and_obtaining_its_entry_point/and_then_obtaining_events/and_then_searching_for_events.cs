using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles.DataModel;
using Heracles.DataModel.Collections;
using Heracles.Namespaces;
using NUnit.Framework;

namespace Having_a_Hydra_client.while_browsing_a_website.and_obtaining_its_entry_point.and_then_obtaining_events
{
    [TestFixture]
    public class and_then_searching_for_events : ScenarioTest
    {
        private IHypermediaContainer SearchedResults { get; set; }

        public override async Task TheTest()
        {
            await base.TheTest();
            var link = Events.Links
                .WithRelationOf(hydra.search)
                .WithTemplate()
                .First()
                .ExpandTarget(new Dictionary<string, string>() { { "searchText", "whatever" } });
            SearchedResults = await Client.GetResource(link);
        }

        [Test]
        public void should_obtain_matching_events()
        {
            SearchedResults.Collections
                .Should()
                .Contain(item => Regex.IsMatch(item.Iri.ToString(), "api/events\\?") && item.Type.Contains(hydra.Collection));
        }
    }
}
