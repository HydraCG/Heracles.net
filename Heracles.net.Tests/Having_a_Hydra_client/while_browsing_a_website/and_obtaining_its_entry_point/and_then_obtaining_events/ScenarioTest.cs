using System;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles.DataModel;
using Heracles.DataModel.Collections;
using NUnit.Framework;
using RDeF.Entities;

namespace Having_a_Hydra_client.while_browsing_a_website.and_obtaining_its_entry_point.and_then_obtaining_events
{
    public abstract class ScenarioTest : and_obtaining_its_entry_point.ScenarioTest
    {
        protected IHypermediaContainer Events { get; private set; }

        public override async Task TheTest()
        {
            await base.TheTest();
            Events = await Client.GetResource(new Uri(Url, "api/events"));
        }

        [Test]
        public void should_obtain_a_collection_of_events()
        {
            Events.Members.OfType(new Iri("http://schema.org/Event")).Should().HaveCount(3);
        }
    }
}
