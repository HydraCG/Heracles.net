using System;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.DataModel.Collections;
using NUnit.Framework;
using RDeF.Entities;

namespace Having_a_Hydra_client.while_browsing_a_website.and_obtaining_its_entry_point.and_then_obtaining_people
{
    public abstract class ScenarioTest : and_obtaining_its_entry_point.ScenarioTest
    {
        protected IHypermediaContainer People { get; set; }

        public override async Task TheTest()
        {
            await base.TheTest();
            People = await Client.GetResource(new Uri(Url, "api/people"));
        }

        [Test]
        public void should_obtain_a_collection_of_people()
        {
            People.Members.OfType(new Iri("http://schema.org/Person")).Should().HaveCount(1);
        }

        [Test]
        public async Task should_follow_all_links_and_gather_all_members()
        {
            (await PartialCollectionCrawler.From(People).GetMembers()).Should().HaveCount(2);
        }
    }
}
