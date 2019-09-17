using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles.DataModel;
using Heracles.DataModel.Collections;
using NUnit.Framework;
using RDeF.Entities;
using Heracles.Testing;

namespace Having_a_Hydra_client.while_browsing_a_website.and_obtaining_its_entry_point.and_then_obtaining_events
{
    [TestFixture]
    public class and_then_using_some_templated_operation : ScenarioTest
    {
        private IHypermediaContainer FilteringResult { get; set; }

        public override async Task TheTest()
        {
            await base.TheTest();
            var link = Events.Links
                .WithRelationOf(new Iri("http://example.com/vocab#filter"))
                .WithTemplate()
                .First()
                .ExpandTarget(_ => _
                    .WithProperty(new Iri("http://schema.org/name")).HavingValueOf("name")
                    .WithVariable("eventDescription").HavingValueOf("description"));
            FilteringResult = await Client.GetResource(link);
        }

        [Test]
        public void should_obtain_matching_events()
        {
            FilteringResult.Should().Contain(new Regex("/api/events\\?"));
        }
    }
}
