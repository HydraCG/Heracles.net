using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles.DataModel;
using Heracles.DataModel.Collections;
using NUnit.Framework;
using RDeF.Entities;

namespace Having_a_Hydra_client.while_browsing_a_website.and_obtaining_its_entry_point
{
    /// <summary>Use case 1. Entry-point.</summary>
    public abstract class ScenarioTest : while_browsing_a_website.ScenarioTest
    {
        protected IHypermediaContainer EntryPoint { get; set; }

        public override async Task TheTest()
        {
            await base.TheTest();
            EntryPoint = await ApiDocumentation.GetEntryPoint();
        }

        [Test]
        public void should_obtain_three_hypermedia_controls()
        {
            EntryPoint.Should().HaveCount(3);
        }

        [Test]
        public void should_obtain_a_schema_AddAction_operations()
        {
            EntryPoint.OfType<IHydraResource>()
                .Count(_ => _.Operations.OfType(new Iri("http://schema.org/AddAction")).Any())
                .Should().Be(2);
        }

        [Test]
        public void it_should_obtain_a_collection_of_events()
        {
            EntryPoint.Collections.FirstOrDefault(item => Regex.IsMatch(item.Iri, "/api/events$"))
                .Should().NotBeNull();
        }
    }
}
